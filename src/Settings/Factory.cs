using System;

namespace Alejof.Netlify.Settings
{
    public class Factory
    {
        public static NetlifySettings Build()
        {
            var getNetlifySetting = GetPrefixedSettingFunc<NetlifySettings>();

            return new NetlifySettings
            {
                ApiBaseUrl = getNetlifySetting(nameof(NetlifySettings.ApiBaseUrl)),
                AccessToken = getNetlifySetting(nameof(NetlifySettings.AccessToken)),
                BuildHooksBaseUrl = getNetlifySetting(nameof(NetlifySettings.BuildHooksBaseUrl)),
            };
        }

        private static Func<string, string> GetPrefixedSettingFunc<T>() =>
            name => Environment.GetEnvironmentVariable($"{typeof(T).Name}_{name}", EnvironmentVariableTarget.Process);
    }
}
