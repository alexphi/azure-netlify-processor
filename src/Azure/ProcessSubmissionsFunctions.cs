using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Alejof.Netlify.Azure
{
    public class ProcessSubmissionsFunctions
    {
        private const string QueueName = "netlify-submission-info";
        private readonly Functions.ISubmissionsFunction _function;

        public ProcessSubmissionsFunctions(
            Functions.ISubmissionsFunction function)
        {
            this._function = function;
        }

        [FunctionName("FetchNetlifySubmissionsOnSchedule")]
        public async Task FetchOnSchedule(
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log,
            [Queue(QueueName)]IAsyncCollector<Models.SubmissionData> dataCollector)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}. Schedule: {myTimer.ScheduleStatus.Next}");

            await _function.FetchSubmissions(dataCollector);
        }

        [FunctionName("RouteNetlifySubmissionsOnQueue")]
        public async Task RouteOnQueue(
            [QueueTrigger(QueueName)]Models.SubmissionData data, ILogger log)
        {
            log.LogInformation($"{nameof(ProcessSubmissionsFunctions)}.{nameof(RouteOnQueue)} function method executing.");
            
            await _function.RouteSubmission(data);
        }
    }
}
