using System;

namespace Alejof.Netlify.Models
{
    public class SubmissionData
    {
        public string Id { get; set; }
        public string SiteUrl { get; set; }
        public string FormName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DataField[] Fields { get; set; }
    }

    public class DataField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
