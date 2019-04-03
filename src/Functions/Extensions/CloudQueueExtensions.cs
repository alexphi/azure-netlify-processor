using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Alejof.Netlify.Functions.Extensions
{
    public static class CloudQueueExtensions
    {
        public static async Task AddMessageAsync<T>(this CloudQueue queue, T message)
        {
            var stringContent = JsonConvert.SerializeObject(message);
            await queue.AddMessageAsync(
                new CloudQueueMessage(stringContent));
        }
    }
}
