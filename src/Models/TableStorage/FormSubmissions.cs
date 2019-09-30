using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Alejof.Netlify.Models.TableStorage
{
    public class FormSubmissionsEntity : TableEntity
    {
        public const string TableName = "NetlifyMappings";
        public const string DefaultKey = "form-submissions";

        public string QueueName { get; set; }
    }
}
