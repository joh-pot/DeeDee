using System.Collections.Generic;
using System.Text;

namespace DeeDee.Builders
{
    internal static class DispatcherClassBuilder
    {
        public static string Build
        (
            string ns,
            List<(string RequestClassName, bool IsAsync)> irequests,
            List<(string RequestClassName, string ResponseClassName, bool IsAsync)> irequestsOfT
        )
        {
            var sourceBuilder = new StringBuilder
            ($@"
                using System;
                using System.Threading;
                using System.Threading.Tasks;
                using System.Linq;
                using DeeDee.Models;
                using ServiceProvider = DeeDee.Models.ServiceProvider;
                namespace {ns}DeeDee.Generated.Models"
            );
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("public class Dispatcher : IDispatcher");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("private readonly ServiceProvider _serviceFactory;");
            PipelineDeclarationsBuilder.LazyDeclarations(ref sourceBuilder, irequests, irequestsOfT);
            sourceBuilder.AppendLine("public Dispatcher(ServiceProvider service)");
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
                    sourceBuilder.AppendFormat
                    (@"
                        public Task SendAsync({0} request, CancellationToken token = default)
                        {{ 
                            var context = new PipelineContext();
                            NextAsync builtPipeline = {1}.Value;
                            return builtPipeline(request, context, token); 
                        }}", requestClassName, PipelineDeclarationsBuilder.SafeVariableName(requestClassName)
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                        public void Send({0} request)
                        {{ 
                            var context = new PipelineContext();
                            Next builtPipeline = {1}.Value;
                            builtPipeline(request, ref context); 
                        }}", requestClassName, PipelineDeclarationsBuilder.SafeVariableName(requestClassName)
                    ).AppendLine();
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
                    sourceBuilder.AppendFormat
                    (@"
                        public Task<{0}> SendAsync
                        (
                            {1} request,
                            CancellationToken token = default
                        )
                        {{ 
                            var context = new PipelineContext<{0}>();
                            NextAsync<{0}> builtPipeline = {2}.Value;
                            return builtPipeline(request, context, token);
                        }}", responseClassName, requestClassName, PipelineDeclarationsBuilder.SafeVariableName(requestClassName, responseClassName)
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                        public {0} Send
                        (
                            {1} request
                        )
                        {{ 
                            var context = new PipelineContext<{0}>();
                            Next<{0}> builtPipeline = {2}.Value;
                            return builtPipeline(request, ref context);
                        }}", responseClassName, requestClassName, PipelineDeclarationsBuilder.SafeVariableName(requestClassName, responseClassName)
                    ).AppendLine();
                }

            }

        }
    }
}
