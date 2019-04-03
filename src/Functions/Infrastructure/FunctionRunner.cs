using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Alejof.Netlify.Functions.Infrastructure
{
    public interface IFunctionRunner<TFunction>
        where TFunction : IFunction
    {
        Task<IActionResult> RunAsync(Func<TFunction, Task<IActionResult>> asyncFunc);
        Task RunAsync(Func<TFunction, Task> asyncFunc);
    }

    public class FunctionRunner<TFunction> : IFunctionRunner<TFunction>
        where TFunction : IFunction
    {
        protected readonly ContainerBuilder _containerBuilder;
        private ILogger _log = null;

        internal FunctionRunner(ILogger log = null)
        {
            _containerBuilder = new ContainerBuilder()
                .RegisterFunction(typeof(TFunction))
                .RegisterSettings();

            _log = log;
        }

        public async Task<IActionResult> RunAsync(Func<TFunction, Task<IActionResult>> asyncFunc)
        {
            var function = BuildFunction(_containerBuilder.Build(), out var result);
            if (function == null)
                return result;

            try
            {
                return await asyncFunc(function)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var errorResult = new ObjectResult(
                    new
                    {
                        type = ex.GetType().FullName,
                        message = ex.Message,
                        source = ex.Source,
                    });
                errorResult.StatusCode = 500;

                return errorResult;
            }
        }

        public async Task RunAsync(Func<TFunction, Task> asyncFunc)
        {
            var function = BuildFunction(_containerBuilder.Build(), out var result);
            if (function == null)
                return;

            await asyncFunc(function)
                .ConfigureAwait(false);
        }

        protected virtual TFunction BuildFunction(IServiceProvider container, out IActionResult result)
        {
            var function = container.GetService<TFunction>();
            if (function == null)
            {
                result = new NotFoundResult();
                return default(TFunction);
            }

            result = null;
            function.Log = _log;

            return function;
        }
    }

    public static class FunctionRunner
    {
        // Factory method
        public static IFunctionRunner<TFunction> Default<TFunction>(ILogger log) where TFunction : IFunction => new FunctionRunner<TFunction>(log: log);
    }
}
