using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Models
{
    public class DeploySignalEntity : TableEntity
    {
        public const string TableName = "NetlifyMappings";
        public const string DefaultKey = "deploy-signal";

        // PartitionKey: "deploy-signal"
        // RowKey: signal name
        
        public string HookId { get; set; }
    }
}
