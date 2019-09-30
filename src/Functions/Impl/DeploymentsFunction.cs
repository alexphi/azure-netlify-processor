
using System.Threading.Tasks;
using Alejof.Netlify.Functions.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace Alejof.Netlify.Functions.Impl
{
    public class DeploymentsFunction : IDeploymentsFunction
    {
        private readonly ILogger _log;
        private readonly Settings.FunctionSettings _settings;
        private readonly CloudStorageAccount _storageAccount;

        public DeploymentsFunction(
            ILogger<DeploymentsFunction> log,
            Settings.FunctionSettings netlifySettings)
        {
            this._log = log;
            this._settings = netlifySettings;

            this._storageAccount = CloudStorageAccount.Parse(_settings.StorageConnectionString);
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
            var table = _storageAccount
                .CreateCloudTableClient()
                .GetTableReference(Models.TableStorage.DeploySignalEntity.TableName);

            var entity = await table.RetrieveAsync<Models.TableStorage.DeploySignalEntity>(Models.TableStorage.DeploySignalEntity.DefaultKey, siteUrl);
            return entity?.HookId;
        }

        private async Task CallNetlifyBuildHook(string hookId)
        {
            try
            {
                var submissions = await _settings.Netlify.BuildHooksBaseUrl
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
