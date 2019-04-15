using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Models
{
    public class SitesEntity : TableEntity
    {
        public const string TableName = "NetlifyMappings";
        public const string DefaultKey = "site";

        // PartitionKey: "site"
        // RowKey: Site name
    }
}
