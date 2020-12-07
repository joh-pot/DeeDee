using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            var dispatcher = provider.GetRequiredService<IDispatcher>();
            await dispatcher.SendAsync(new GetARequest()); 
        }
    }


    internal class GetARequest : IRequest<List<List<string>>>
    {

    }

    internal class HandlerA : IPipelineActionAsync<GetARequest, List<List<string>>>
    {
        public Task<List<List<string>>> InvokeAsync(GetARequest request, PipelineContext<List<List<string>>> context, NextAsync<List<List<string>>> next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
