using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Models
{
    public class FormSiteEntity : TableEntity
    {
        public const string TableName = "NetlifyMappings";
        public const string DefaultKey = "forms-site";

        // PartitionKey: "site"
        // RowKey: Site name
    }
}
