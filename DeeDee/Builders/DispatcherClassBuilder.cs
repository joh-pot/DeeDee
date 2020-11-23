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
            sourceBuilder.AppendLine("public class Dispatcher : IDispatcher");
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
            foreach (var (RequestClassName, IsAsync) in irequests)
            {
                if (IsAsync)
                {
                    sourceBuilder.AppendLine
                    (@$"
                        public Task SendAsync({RequestClassName} request, CancellationToken token = default)
                        {{ 
                            var context = new PipelineContext();
                            NextAsync builtPipeline = {PipelineDeclarationsBuilder.SafeVariableName(RequestClassName)}.Value;
                            return builtPipeline(request, context, token); 
                        }}"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                        public void Send({RequestClassName} request)
                        {{ 
                            var context = new PipelineContext();
                            Next builtPipeline = {PipelineDeclarationsBuilder.SafeVariableName(RequestClassName)}.Value;
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

            foreach (var (RequestClassName, ResponseClassName, IsAsync) in irequestsOfT)
            {
                if (IsAsync)
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public Task<{ResponseClassName}> SendAsync
                        (
                            {RequestClassName} request,
                            CancellationToken token = default
                        )
                        {{ 
                            var context = new PipelineContext<{ResponseClassName}>();
                            NextAsync<{ResponseClassName}> builtPipeline = {PipelineDeclarationsBuilder.SafeVariableName(RequestClassName, ResponseClassName)}.Value;
                            return builtPipeline(request, context, token);
                        }}"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public {ResponseClassName} Send
                        (
                            {RequestClassName} request
                        )
                        {{ 
                            var context = new PipelineContext<{ResponseClassName}>();
                            Next<{ResponseClassName}> builtPipeline = {PipelineDeclarationsBuilder.SafeVariableName(RequestClassName, ResponseClassName)}.Value;
                            builtPipeline(request, ref context);
                        }}"
                    );
                }

            }

        }
    }
}
