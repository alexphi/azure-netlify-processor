using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Models.TableStorage
{
    public class DeploySignalEntity : TableEntity
    {
        public const string TableName = "NetlifyMappings";
        public const string DefaultKey = "deploy-signal";
        
        public string HookId { get; set; }
    }
}
