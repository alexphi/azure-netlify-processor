using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Models
{
    public class SubmissionMappingEntity : TableEntity
    {
        public const string TableName = "NetlifySubmissionMappings";

        // PartitionKey: SiteUrl
        // RowKey: FormName

        public string QueueName { get; set; }
    }
}
