using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Alejof.Netlify.Models;
using Alejof.Netlify.Settings;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Alejof.Netlify.Functions
{
    public static class FetchSubmissions
    {
        [FunctionName("FetchSubmissions")]
        public static async Task Run(
            [TimerTrigger("0 0 7,12,17 * * *")]TimerInfo myTimer, ILogger log,
            [Queue(Queues.SubmissionInfo)]IAsyncCollector<Models.SubmissionData> dataCollector)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var settings = Settings.Factory.BuildFunctionSettings();
            var httpClient = new HttpClient();

            // 1. Fetch Netlyfy Sites
            var sites = await GetNetlifySiteIds(httpClient, settings, log);

            // 2. Foreach site, fetch submissions
            foreach (var site in sites)
            {
                var submissions = await GetNetlifySubmissions(httpClient, settings, log, site);

                // 3. Enqueue Submission data
                foreach (var s in submissions)
                    await dataCollector.AddAsync(s);
            }
        }

        private static async Task<IEnumerable<string>> GetNetlifySiteIds(HttpClient httpClient, FunctionSettings settings, ILogger log)
        {
            var requestUrl = $"{settings.NetlifyBaseUrl}/sites?access_token={settings.NetlifyAccessToken}";
            var response = await httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                log.LogWarning("No Netlify sites found");
                return Enumerable.Empty<string>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var sites = JArray.Parse(content);

            return sites.Select((dynamic d) => (string)d.site_id);
        }

        private static async Task<IEnumerable<SubmissionData>> GetNetlifySubmissions(HttpClient httpClient, FunctionSettings settings, ILogger log, string site)
        {
            var requestUrl = $"{settings.NetlifyBaseUrl}/sites/{site}/submissions?access_token={settings.NetlifyAccessToken}";
            var response = await httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                log.LogWarning($"No Netlify Form submissions found for site {site}");
                return Enumerable.Empty<SubmissionData>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var submissions = JArray.Parse(content);

            return submissions
                .Select(
                    (dynamic s) => new Models.SubmissionData
                    {
                        Fields = ((IEnumerable)s.ordered_human_fields)
                            .Cast<dynamic>()
                            .Select(
                                field => new Models.DataField
                                {
                                    Name = field.name,
                                    Title = field.Title,
                                    Value = field.value,
                                })
                            .ToArray()
                    });
        }
    }
}
