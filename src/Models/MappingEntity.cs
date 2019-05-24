using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Models
{
    public class MappingEntity : TableEntity
    {
        public const string TableName = "NetlifyMappings";
        public const string DefaultKey = "forms";

        // PartitionKey: SiteUrl
        // RowKey: FormName

        public string QueueName { get; set; }
    }
}
