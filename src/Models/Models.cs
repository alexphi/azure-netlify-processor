using System;
using System.Linq;

namespace Alejof.Netlify.Models
{
    public class SubmissionData
    {
        public string Id { get; set; }
        public string SiteUrl { get; set; }
        public string FormName { get; set; }
        public DateTime CreatedAt { get; set; }
        public SubmissionField[] Fields { get; set; }
    }

    public class SubmissionField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
