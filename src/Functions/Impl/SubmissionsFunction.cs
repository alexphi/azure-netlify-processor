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

        public SubmissionsFunction(
            ILogger log,
            Settings.FunctionSettings netlifySettings)
        {
            this._log = log;
            this._settings = netlifySettings;
        }

        public async Task FetchSubmissions(IAsyncCollector<Models.SubmissionData> dataCollector)
        {
            // 2. Foreach site, fetch submissions
            foreach (var site in _settings.Netlify.Sites.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var submissions = await GetNetlifySubmissions(site);

                // 3. Enqueue Submission data
                foreach (var s in submissions)
                    await dataCollector.AddAsync(s);
            }
        }

        private async Task<IEnumerable<Models.SubmissionData>> GetNetlifySubmissions(string site)
        {
            try
            {
                var submissions = await _settings.Netlify.BaseUrl
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

        public async Task RouteSubmission(Models.SubmissionData data)
        {
            // Map site url and form name to specialized queue name - if match, then queue info
            var storageAccount = CloudStorageAccount.Parse(_settings.StorageConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(Tables.Mapping);
            var mapping = await table.RetrieveAsync<Models.SubmissionMappingEntity>(data.SiteUrl, data.FormName);

            if (mapping == null)
            {
                _log.LogWarning($"No queue mapping found for {data.SiteUrl}, form {data.FormName}");
                return;
            }

            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(mapping.QueueName);
            await queue.CreateIfNotExistsAsync();
            await queue.AddMessageAsync(data);

            // Delete submission
            await DeleteNetlifySubmission(data.Id);
        }

        private async Task DeleteNetlifySubmission(string id)
        {
            try
            {
                var submissions = await _settings.Netlify.BaseUrl
                    .AppendPathSegments("submissions", id)
                    .SetQueryParam("access_token", _settings.Netlify.AccessToken)
                    .DeleteAsync();
            }
            catch (FlurlHttpException ex)
            {
                _log.LogError(ex.Message);
            }
        }
    }
}
