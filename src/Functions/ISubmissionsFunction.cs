using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Alejof.Netlify.Functions
{
    public interface ISubmissionsFunction
    {
        Task FetchSubmissions(IAsyncCollector<Models.SubmissionData> dataCollector);
        Task RouteSubmission(Models.SubmissionData data);
    }
}
