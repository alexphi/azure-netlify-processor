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
                StorageConnectionString = GetSetting("AzureWebJobsStorage"),

                Netlify = new NetlifySettings
                {
                    BaseUrl = getNetlifySetting("BaseUrl"),
                    AccessToken = getNetlifySetting("AccessToken"),
                }
            };
        }

        private static string GetSetting(string name) =>
            Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        private static Func<string, string> GetPrefixedSettingFunc<T>() =>
            name => Environment.GetEnvironmentVariable($"{typeof(T).Name}_{name}", EnvironmentVariableTarget.Process);
    }
}
