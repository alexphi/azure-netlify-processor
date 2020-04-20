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
                StorageConnectionString = GetSetting("StorageConnectionString"),
                DefaultQueueName = "netlify-contact-info",

                Netlify = new NetlifySettings
                {
                    ApiBaseUrl = getNetlifySetting("ApiBaseUrl"),
                    AccessToken = getNetlifySetting("AccessToken"),
                    BuildHooksBaseUrl = getNetlifySetting("BuildHooksBaseUrl"),
                }
            };
        }

        private static string GetSetting(string name) =>
            Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        private static Func<string, string> GetPrefixedSettingFunc<T>() =>
            name => Environment.GetEnvironmentVariable($"{typeof(T).Name}_{name}", EnvironmentVariableTarget.Process);
    }
}
