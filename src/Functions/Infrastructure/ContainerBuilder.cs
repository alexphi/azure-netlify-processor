using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Alejof.Netlify.Functions.Infrastructure
{
    public class ContainerBuilder
    {
        private readonly IServiceCollection _services;

        internal ContainerBuilder()
        {
            this._services = new ServiceCollection();
        }

        internal ContainerBuilder RegisterFunction(Type type)
        {
            this._services.AddTransient(type);
            return this;
        }

        internal ContainerBuilder RegisterSettings()
        {
            this._services.AddScoped<Settings.FunctionSettings>(svc => Settings.Factory.BuildFunctionSettings());
            return this;
        }

        public IServiceProvider Build() => this._services.BuildServiceProvider();
    }
}
