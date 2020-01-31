namespace Alejof.Netlify.Settings
{
    public class FunctionSettings
    {
        public string StorageConnectionString { get; set; }
        public string DefaultQueueName { get; set; }

        public NetlifySettings Netlify { get; set; }
    }

    public class NetlifySettings
    {
        public string ApiBaseUrl { get; set; }
        public string BuildHooksBaseUrl { get; set; }
        public string AccessToken { get; set; }
    }
}
