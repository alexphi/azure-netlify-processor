using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Flurl;
using Flurl.Http;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Alejof.Netlify.Functions.Infrastructure;

namespace Alejof.Netlify.Functions.Impl
{
    public class SubmissionsFunction : IFunction
    {
        private readonly Settings.FunctionSettings _settings;

        public ILogger Log { get; set; }

        public SubmissionsFunction(Settings.FunctionSettings settings)
        {
            this._settings = settings;
        }

        public async Task FetchSubmissions(IAsyncCollector<Models.SubmissionData> dataCollector)
        {
            // 1. Fetch Netlyfy Sites
            var sites = await GetNetlifySiteIds();

            // 2. Foreach site, fetch submissions
            foreach (var site in sites)
            {
                var submissions = await GetNetlifySubmissions(site);

                // 3. Enqueue Submission data
                foreach (var s in submissions)
                    await dataCollector.AddAsync(s);
            }
        }

        private async Task<IEnumerable<string>> GetNetlifySiteIds()
        {
            try
            {
                var sites = await _settings.NetlifyBaseUrl
                    .AppendPathSegments("sites")
                    .SetQueryParam("access_token", _settings.NetlifyAccessToken)
                    .GetJsonListAsync();

                return sites
                    .Select(d => (string)d.site_id);
            }
            catch (FlurlHttpException ex)
            {
                Log.LogError(ex.Message);
                return Enumerable.Empty<string>();
            }
        }

        private async Task<IEnumerable<Models.SubmissionData>> GetNetlifySubmissions(string site)
        {
            try
            {
                var submissions = await _settings.NetlifyBaseUrl
                    .AppendPathSegments("sites", site, "submissions")
                    .SetQueryParam("access_token", _settings.NetlifyAccessToken)
                    .GetJsonListAsync();

                return submissions
                    .Select(
                        s => new Models.SubmissionData
                        {
                            Id = s.id,
                            SiteUrl = s.site_url,
                            FormName = s.form_name,
                            CreatedAt = s.created_at,
                            Fields = ((IEnumerable)s.ordered_human_fields)
                                .Cast<dynamic>()
                                .Select(
                                    field => new Models.DataField
                                    {
                                        Name = field.name,
                                        Value = field.value,
                                    })
                                .ToArray()
                        });
            }
            catch (FlurlHttpException ex)
            {
                Log.LogError(ex.Message);
                return Enumerable.Empty<Models.SubmissionData>();
            }
        }
    }
}
