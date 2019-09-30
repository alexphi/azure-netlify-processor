using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Alejof.Netlify.Functions
{
    public interface IDeploymentsFunction
    {
        Task DeploySite(string deploySignal);
    }
}
