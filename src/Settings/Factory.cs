using System;

namespace Alejof.Netlify.Settings
{
    public class Factory
    {
        public static FunctionSettings Build()
        {
            var getNetlifySetting = GetPrefixedSettingFunc<NetlifySettings>();

            return new FunctionSettings
            {
                HostingConnectionString = GetSetting("AzureWebJobsStorage"),
                StorageConnectionString = GetSetting(nameof(FunctionSettings.StorageConnectionString)),

                Netlify = new NetlifySettings
                {
                    ApiBaseUrl = getNetlifySetting(nameof(NetlifySettings.ApiBaseUrl)),
                    AccessToken = getNetlifySetting(nameof(NetlifySettings.AccessToken)),
                    BuildHooksBaseUrl = getNetlifySetting(nameof(NetlifySettings.BuildHooksBaseUrl)),
                }
            };
        }

        private static string GetSetting(string name) =>
            Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        private static Func<string, string> GetPrefixedSettingFunc<T>() =>
            name => Environment.GetEnvironmentVariable($"{typeof(T).Name}_{name}", EnvironmentVariableTarget.Process);
    }
}
