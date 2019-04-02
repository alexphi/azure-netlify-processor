namespace Alejof.Netlify.Models
{
    public class SubmissionData
    {
        public DataField[] Fields { get; set; }
    }

    public class DataField
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
