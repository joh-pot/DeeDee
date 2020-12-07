using System.Collections.Generic;
using System.Text;

namespace DeeDee.Builders
{
    internal static class DispatcherClassBuilder
    {
        public static string Build
        (
            List<(string RequestClassName, bool IsAsync)> irequests,
            List<(string RequestClassName, string ResponseClassName, bool IsAsync)> irequestsOfT
        )
        {
            var sourceBuilder = new StringBuilder
            (@"
                using System;
                using System.Threading;
                using System.Threading.Tasks;
                using System.Linq;
                using DeeDee.Models;
                namespace DeeDee"
            );
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("internal class Dispatcher : IDispatcher");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("private readonly DeeDee.Models.ServiceProvider _serviceFactory;");
            PipelineDeclarationsBuilder.LazyDeclarations(ref sourceBuilder, irequests, irequestsOfT);
            sourceBuilder.AppendLine(@"public Dispatcher(DeeDee.Models.ServiceProvider service)");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("_serviceFactory = service;");
            PipelineDeclarationsBuilder.LazyInitializations(ref sourceBuilder, irequests, irequestsOfT);
            sourceBuilder.AppendLine("}");
            MethodsIRequest(ref sourceBuilder, irequests);
            MethodsIRequestT(ref sourceBuilder, irequestsOfT);

            PipelineDeclarationsBuilder.LazyFactoryMethods(ref sourceBuilder);

            sourceBuilder.AppendLine("}");

            sourceBuilder.AppendLine("}");
            return sourceBuilder.ToString();
        }

        private static void MethodsIRequest
        (
            ref StringBuilder sourceBuilder,
            List<(string RequestClassName, bool isAsync)> irequests
        )
        {
            foreach (var (requestClassName, isAsync) in irequests)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendLine
                    (@$"
                        public Task SendAsync({requestClassName} request, CancellationToken token = default)
                        {{ 
                            var context = new PipelineContext();
                            NextAsync builtPipeline = {PipelineDeclarationsBuilder.SafeVariableName(requestClassName)}.Value;
                            return builtPipeline(request, context, token); 
                        }}"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                        public void Send({requestClassName} request)
                        {{ 
                            var context = new PipelineContext();
                            Next builtPipeline = {PipelineDeclarationsBuilder.SafeVariableName(requestClassName)}.Value;
                            builtPipeline(request, ref context); 
                        }}"
                    );
                }

            }
        }

        private static void MethodsIRequestT
        (
            ref StringBuilder sourceBuilder,
            List<(string RequestClassName, string ResponseClassName, bool IsAsync)> irequestsOfT
        )
        {

            foreach (var (requestClassName, responseClassName, isAsync) in irequestsOfT)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public Task<{responseClassName}> SendAsync
                        (
                            {requestClassName} request,
                            CancellationToken token = default
                        )
                        {{ 
                            var context = new PipelineContext<{responseClassName}>();
                            NextAsync<{responseClassName}> builtPipeline = {PipelineDeclarationsBuilder.SafeVariableName(requestClassName, responseClassName)}.Value;
                            return builtPipeline(request, context, token);
                        }}"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public {responseClassName} Send
                        (
                            {requestClassName} request
                        )
                        {{ 
                            var context = new PipelineContext<{responseClassName}>();
                            Next<{responseClassName}> builtPipeline = {PipelineDeclarationsBuilder.SafeVariableName(requestClassName, responseClassName)}.Value;
                            builtPipeline(request, ref context);
                        }}"
                    );
                }

            }

        }
    }
}
