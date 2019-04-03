using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Alejof.Netlify.Functions.Infrastructure;
using Alejof.Netlify.Models;
using Alejof.Netlify.Settings;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Alejof.Netlify.Functions
{
    public static class NetlifySubmissions
    {
        [FunctionName("FetchNetlifySubmissionsOnSchedule")]
        public static async Task RunOnSchedule(
            [TimerTrigger("0 0 7,12,17 * * *")]TimerInfo myTimer, ILogger log,
            [Queue(Queues.SubmissionInfo)]IAsyncCollector<Models.SubmissionData> dataCollector)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            await FunctionRunner.Default<Impl.SubmissionsFunction>(log)
                .RunAsync(f => f.FetchSubmissions(dataCollector));
        }
    }
}
