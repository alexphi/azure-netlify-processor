using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Models.TableStorage
{
    /// <summary>
    /// PartitionKey: "submissions-routing", RowKey: {siteUrl}-{formName}
    /// </summary>
    public class MappingsEntity : TableEntity
    {
        public const string TableName = "NetlifyMappings";
        public const string DefaultKey = "submission-routing";

        public string QueueNames { get; set; }

        public string SiteName => RowKey.Substring(0, RowKey.LastIndexOf("-"));
        public string FormName => RowKey.Substring(RowKey.LastIndexOf("-") + 1);
    }

    public class DeploySignalEntity : TableEntity
    {
        public const string TableName = "NetlifyMappings";
        public const string DefaultKey = "deploy-signal";
        
        public string HookId { get; set; }
    }
}
