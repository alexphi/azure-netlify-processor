
using System.Threading.Tasks;
using Alejof.Netlify.Functions.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Functions.Impl
{
    public class DeploymentsFunction : IDeploymentsFunction
    {
        private readonly ILogger _log;
        private readonly Settings.NetlifySettings _settings;
        private readonly CloudTableClient _cloudTableClient;

        public DeploymentsFunction(
            ILogger<DeploymentsFunction> log,
            Settings.NetlifySettings netlifySettings,
            CloudTableClient cloudTableClient)
        {
            _log = log;
            _settings = netlifySettings;
            _cloudTableClient = cloudTableClient;
        }
        
        public async Task DeploySite(string deploySignal)
        {
            var hookId = await GetDeployHookId(deploySignal);
            if (string.IsNullOrEmpty(hookId))
            {
                _log.LogWarning($"No buildHookId found for {deploySignal}");
                return;
            }

            await CallNetlifyBuildHook(hookId);
        }

        private async Task<string> GetDeployHookId(string siteUrl)
        {
            var table = _cloudTableClient.GetTableReference(Models.TableStorage.DeploySignalEntity.TableName);
            await table.CreateIfNotExistsAsync();

            var entity = await table.RetrieveAsync<Models.TableStorage.DeploySignalEntity>(Models.TableStorage.DeploySignalEntity.DefaultKey, siteUrl);
            return entity?.HookId;
        }

        private async Task CallNetlifyBuildHook(string hookId)
        {
            try
            {
                var submissions = await _settings.BuildHooksBaseUrl
                    .AppendPathSegments(hookId)
                    .PostAsync(null);
            }
            catch (FlurlHttpException ex)
            {
                _log.LogError(ex.Message);
            }
        }
    }
}
