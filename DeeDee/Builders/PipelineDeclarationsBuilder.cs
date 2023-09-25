using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeeDee.Builders
{
    internal static class PipelineDeclarationsBuilder
    {
        public static void LazyDeclarations
        (
            ref StringBuilder sourceBuilder,
            List<(string RequestClassName, bool IsAsync)> irequests,
            List<(string RequestClassName, string ResponseClassName, bool IsAsync)> irequestsOfT
        )
        {
            foreach (var (requestClassName, isAsync) in irequests)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendFormat
                    (@"
                        private readonly Lazy<NextAsync> {0};", SafeVariableName(requestClassName)
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                        private readonly Lazy<Next> {0};", SafeVariableName(requestClassName)
                    ).AppendLine();
                }
            }

            foreach (var (requestClassName, responseClassName, isAsync) in irequestsOfT)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendFormat
                    (@"
                        private readonly Lazy<NextAsync<{0}>> {1};", responseClassName, SafeVariableName(requestClassName, responseClassName)
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                        private readonly Lazy<Next<{0}>> {1};", responseClassName, SafeVariableName(requestClassName, responseClassName)
                    ).AppendLine();
                }
            }
        }

        public static void LazyInitializations
        (
            ref StringBuilder sourceBuilder,
            List<(string RequestClassName, bool IsAsync)> irequests,
            List<(string RequestClassName, string ResponseClassName, bool IsAsync)> irequestsOfT
        )
        {
            foreach (var (requestClassName, isAsync) in irequests)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendFormat
                    (@"
                        {0} = new Lazy<NextAsync>(BuildAsync<{1}>);", SafeVariableName(requestClassName), requestClassName
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                        {0} = new Lazy<Next>(Build<{1}>);", SafeVariableName(requestClassName), requestClassName
                    ).AppendLine();
                }
            }

            foreach (var (requestClassName, responseClassName, isAsync) in irequestsOfT)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendFormat
                    (@"
                        {0} = new Lazy<NextAsync<{1}>>(BuildAsync<{2},{1}>);", SafeVariableName(requestClassName, responseClassName), responseClassName, requestClassName
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                       {0} = new Lazy<Next<{1}>>(Build<{2},{1}>);", SafeVariableName(requestClassName, responseClassName), responseClassName, requestClassName
                    ).AppendLine();
                }
            }
        }


        public static void LazyFactoryMethods(ref StringBuilder sourceBuilder)
        {

            sourceBuilder.AppendLine
            (@"
                private NextAsync BuildAsync<TRequest>() where TRequest : IRequest
                {{
                    var actions = _serviceFactory.GetServices<IPipelineActionAsync<TRequest>>();

                    var builtPipeline = actions.Aggregate
                    (
                        (NextAsync)((req, ctx, tkn) => Task.CompletedTask),
                        (next, pipeline) => (req, ctx, tkn) => pipeline.InvokeAsync((TRequest)req, ctx, next, tkn)
                    );

                    return builtPipeline;
                }}"
            );

            sourceBuilder.AppendLine
            (@"
                private Next Build<TRequest>() where TRequest : IRequest
                {{
                    var actions = _serviceFactory.GetServices<IPipelineAction<TRequest>>();

                    var builtPipeline = actions.Aggregate
                    (
                        (Next)((IRequest req, ref PipelineContext ctx) => {{ }}),
                        (next, pipeline) =>
                            (IRequest req, ref PipelineContext ctx) =>
                                pipeline.Invoke((TRequest)req, ref ctx, next)
                    );

                    return builtPipeline;
                }}"
            );


            sourceBuilder.AppendLine
            (@"
                private NextAsync<TResponse> BuildAsync<TRequest, TResponse>() where TRequest : IRequest<TResponse>
                {{
                    var actions = _serviceFactory.GetServices<IPipelineActionAsync<TRequest, TResponse>>();

                    var builtPipeline = actions.Aggregate
                    (
                        (NextAsync<TResponse>)((req, ctx, tkn) => Task.FromResult(ctx.Result)),
                        (next, pipeline) => (req, ctx, tkn) => pipeline.InvokeAsync((TRequest)req, ctx, next, tkn)
                    );

                    return builtPipeline;
                }}"
            );

            sourceBuilder.AppendLine
            (@"
                private Next<TResponse> Build<TRequest, TResponse>() where TRequest : IRequest<TResponse>
                {{
                    var actions = _serviceFactory.GetServices<IPipelineAction<TRequest, TResponse>>();

                    var builtPipeline = actions.Aggregate
                    (
                        (Next<TResponse>)((IRequest<TResponse> req, ref PipelineContext<TResponse> ctx) => ctx.Result),
                        (next, pipeline) =>
                            (IRequest<TResponse> req, ref PipelineContext<TResponse> ctx) =>
                                pipeline.Invoke((TRequest)req, ref ctx, next)
                    );

                    return builtPipeline;
                }}"
            );
        }

        private static readonly Regex Safe = new("[^a-z]", RegexOptions.Compiled| RegexOptions.IgnoreCase);
        public static string SafeVariableName(string requestClassName)
        {
            return $"_{Safe.Replace(requestClassName, "_")}_lazy";
        }

        public static string SafeVariableName(string requestClassName, string responseClassName)
        {
            return $"_{Safe.Replace(requestClassName, "_")}_{Safe.Replace(responseClassName, "_")}_lazy";
        }

    }
}
