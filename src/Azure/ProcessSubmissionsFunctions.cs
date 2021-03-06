using System;
using System.Threading.Tasks;
using Alejof.Netlify.Functions.Extensions;
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
            [TimerTrigger("0 */10 * * * *")]TimerInfo myTimer, ILogger log,
            [Queue(QueueName, Connection = Storage.ConnectionStringSetting)]IAsyncCollector<Models.SubmissionData> dataCollector)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}. Schedule: {myTimer.ScheduleStatus.Next}");

            await _function.FetchSubmissions(dataCollector);
        }

        [FunctionName("RouteNetlifySubmissionsOnQueue")]
        public async Task RouteOnQueue(
            [QueueTrigger(QueueName, Connection = Storage.ConnectionStringSetting)]Models.SubmissionData data, ILogger log)
        {
            log.LogInformation($"{nameof(ProcessSubmissionsFunctions)}.{nameof(RouteOnQueue)} function method executing.");
            
            await _function.RouteSubmission(data);
        }
    }
}
