using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Alejof.Netlify.Functions
{
    public static class NetlifySubmissions
    {
        [FunctionName("FetchNetlifySubmissionsOnSchedule")]
        public static async Task FetchOnSchedule(
            [TimerTrigger("0 0 12,17,22 * * *")]TimerInfo myTimer, ILogger log,
            [Queue(Queues.Submissions)]IAsyncCollector<Models.SubmissionData> dataCollector)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            await BuildFunctionImpl(log)
                .FetchSubmissions(dataCollector);
        }

        [FunctionName("RouteNetlifySubmissionsOnQueue")]
        public static async Task RouteOnQueue(
            [QueueTrigger(Queues.Submissions)]Models.SubmissionData data, ILogger log)
        {
            await BuildFunctionImpl(log)
                .RouteSubmission(data);
        }

        public static Impl.SubmissionsFunction BuildFunctionImpl(ILogger log) => new Impl.SubmissionsFunction(log, Settings.Factory.Build());
    }
}
