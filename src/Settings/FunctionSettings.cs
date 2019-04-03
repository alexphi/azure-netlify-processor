namespace Alejof.Netlify.Settings
{
    public class FunctionSettings
    {
        public string StorageConnectionString { get; set; }

        public NetlifySettings Netlify { get; set; }
    }

    public class NetlifySettings
    {
        public string BaseUrl { get; set; }
        public string AccessToken { get; set; }
        public string Sites { get; set; }
    }
}
