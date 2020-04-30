using System;
using System.Threading.Tasks;
using Alejof.Netlify.Functions.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Alejof.Netlify.Azure
{
    public class DeploySiteFunction
    {
        private const string QueueName = "netlify-deploy-signal";
        
        private readonly Functions.IDeploymentsFunction _function;

        public DeploySiteFunction(
            Functions.IDeploymentsFunction function)
        {
            this._function = function;
        }

        [FunctionName("DeployNetlifySiteOnQueue")]
        public async Task Run(
            [QueueTrigger(QueueName, Connection = Storage.ConnectionStringSetting)]string deploySignal, ILogger log)
        {
            log.LogInformation($"{nameof(DeploySiteFunction)}.{nameof(Run)} function method executing.");

            await _function.DeploySite(deploySignal);
        }
    }
}
