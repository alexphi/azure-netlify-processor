using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Alejof.Netlify.Functions.Extensions;

namespace Alejof.Netlify.Functions.Impl
{
    public class SubmissionsFunction
    {
        private readonly ILogger _log;
        private readonly Settings.FunctionSettings _settings;
        private readonly CloudStorageAccount _storageAccount;

        public SubmissionsFunction(
            ILogger log,
            Settings.FunctionSettings netlifySettings)
        {
            this._log = log;
            this._settings = netlifySettings;

            this._storageAccount = CloudStorageAccount.Parse(_settings.StorageConnectionString);
        }

        public async Task FetchSubmissions(IAsyncCollector<Models.SubmissionData> dataCollector)
        {
            var sites = await GetNetlifySites();
            
            // 2. Foreach site, fetch submissions
            foreach (var site in sites)
            {
                _log.LogInformation($"Fetching submissions from Netlify, site {site}");
                var submissions = await GetNetlifySubmissions(site);

                // 3. Enqueue Submission data
                foreach (var s in submissions)
                    await dataCollector.AddAsync(s);
            }
        }

        public async Task RouteSubmission(Models.SubmissionData data)
        {
            // Map site url and form name to specialized queue name - if match, then queue info
            var targetQueue = await GetTargetQueueName(data.SiteUrl, data.FormName);
            if (string.IsNullOrEmpty(targetQueue))
            {
                _log.LogWarning($"No queue mapping found for {data.SiteUrl}/{data.FormName}");
                return;
            }

            await EnqueueSubmission(data, targetQueue);

            // Delete submission
            _log.LogInformation($"Routed submission {data.Id} for {data.SiteUrl}/{data.FormName}. Deleting submission from Netlify");
            await DeleteNetlifySubmission(data.Id);
        }

        private async Task<string[]> GetNetlifySites()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(Models.FormSiteEntity.TableName);

            var sites = await table.ScanAsync<Models.FormSiteEntity>(Models.FormSiteEntity.DefaultKey);
            return sites
                .Select(s => s.RowKey)
                .ToArray();
        }

        private async Task<IEnumerable<Models.SubmissionData>> GetNetlifySubmissions(string site)
        {
            try
            {
                var submissions = await _settings.Netlify.ApiBaseUrl
                    .AppendPathSegments("sites", site, "submissions")
                    .SetQueryParam("access_token", _settings.Netlify.AccessToken)
                    .GetJsonListAsync();

                return submissions
                    .Select(
                        s => new Models.SubmissionData
                        {
                            Id = s.id,
                            FormName = s.form_name,
                            SiteUrl = ((string)s.site_url)
                                .Replace("http://", string.Empty)
                                .Replace("https://", string.Empty),
                            CreatedAt = s.created_at,
                            Fields = ((IEnumerable)s.ordered_human_fields)
                                .Cast<dynamic>()
                                .Select(
                                    field => new Models.SubmissionField
                                    {
                                        Name = field.name,
                                        Value = field.value,
                                    })
                                .ToArray()
                        });
            }
            catch (FlurlHttpException ex)
            {
                _log.LogError(ex.Message);
                return Enumerable.Empty<Models.SubmissionData>();
            }
        }

        private async Task DeleteNetlifySubmission(string id)
        {
            try
            {
                var submissions = await _settings.Netlify.ApiBaseUrl
                    .AppendPathSegments("submissions", id)
                    .SetQueryParam("access_token", _settings.Netlify.AccessToken)
                    .DeleteAsync();
            }
            catch (FlurlHttpException ex)
            {
                _log.LogError(ex.Message);
            }
        }
        
        private async Task<string> GetTargetQueueName(string siteUrl, string formName)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(Models.MappingEntity.TableName);

            var mapping = await table.RetrieveAsync<Models.MappingEntity>(Models.MappingEntity.DefaultKey, $"{siteUrl}_{formName}");
            return mapping?.QueueName;
        }
        
        private async Task EnqueueSubmission(Models.SubmissionData data, string queueName)
        {
            var queueClient = _storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            await queue.CreateIfNotExistsAsync();
            await queue.AddMessageAsync(data);
        }
    }
}
