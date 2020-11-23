using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using DeeDee;
using DeeDee.Models;
using System.Threading;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDispatcher();
            var provider = serviceCollection.BuildServiceProvider();
            var dispatcher = provider.GetService<IDispatcher>();
            await dispatcher.SendAsync(new GetARequest());
        }
    }


    public class GetARequest : IRequest
    {

    }

    public class HandlerA : IPipelineActionAsync<GetARequest>
    {
        public Task InvokeAsync(GetARequest request, PipelineContext context, NextAsync next, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
