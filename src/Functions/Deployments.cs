using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Alejof.Netlify.Functions
{
    public static class Deployments
    {
        private const string QueueName = "netlify-deploy-signal";

        [FunctionName("DeployNetlifySiteOnQueue")]
        public static async Task DeployOnQueue(
            [QueueTrigger(QueueName)]string deploySignal, ILogger log)
        {
            await BuildFunctionImpl(log)
                .DeploySite(deploySignal);
        }

        public static Impl.DeploymentsFunction BuildFunctionImpl(ILogger log) => new Impl.DeploymentsFunction(log, Settings.Factory.Build());
    }
}
