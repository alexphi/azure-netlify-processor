using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace Alejof.Netlify.Functions.Extensions
{
    public static class Storage
    {
        public const string ConnectionStringSetting = "StorageConnectionString";
    }

    public static class ServiceExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            // Azure Storage
            var connectionString = System.Environment.GetEnvironmentVariable(Storage.ConnectionStringSetting, EnvironmentVariableTarget.Process);

            services.AddSingleton(svc =>
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                return storageAccount.CreateCloudTableClient();
            });
            
            services.AddSingleton(svc =>
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                return storageAccount.CreateCloudQueueClient();
            });

            return services;
        }
    }
}