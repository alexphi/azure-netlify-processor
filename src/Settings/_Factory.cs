using System;

namespace Alejof.Netlify.Settings
{
    public class Factory
    {
        public static FunctionSettings BuildFunctionSettings()
        {
            var getSetting = GetSettingFunc<FunctionSettings>();

            return new FunctionSettings
            {
                NetlifyBaseUrl = getSetting("NetlifyBaseUrl"),
                NetlifyAccessToken = getSetting("NetlifyAccessToken"),
            };
        }

        private static Func<string, string> GetSettingFunc<T>() => (
            name => Environment.GetEnvironmentVariable($"{typeof(T).Name}_{name}", EnvironmentVariableTarget.Process));
    }
}
