using Microsoft.Extensions.Logging;

namespace Alejof.Netlify.Functions.Infrastructure
{
    public interface IFunction
    {
        // Marker interface for infrastructure classes
        ILogger Log { get; set; }
    }
}
