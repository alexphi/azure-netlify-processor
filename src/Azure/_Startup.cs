using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Alejof.Netlify.Azure.Startup))]

namespace Alejof.Netlify.Azure
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped(s => Settings.Factory.Build());

            builder.Services.AddScoped<Functions.ISubmissionsFunction, Functions.Impl.SubmissionsFunction>();
            builder.Services.AddScoped<Functions.IDeploymentsFunction, Functions.Impl.DeploymentsFunction>();
        }
    }
}
