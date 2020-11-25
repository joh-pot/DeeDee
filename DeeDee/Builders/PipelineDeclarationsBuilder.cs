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
                    sourceBuilder.AppendLine
                    (@$"
                        private readonly Lazy<NextAsync> {SafeVariableName(requestClassName)};"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                        private readonly Lazy<Next> {SafeVariableName(requestClassName)};"
                    );
                }
            }

            foreach (var (requestClassName, responseClassName, isAsync) in irequestsOfT)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendLine
                    (@$"
                        private readonly Lazy<NextAsync<{responseClassName}>> {SafeVariableName(requestClassName, responseClassName)};"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                        private readonly Lazy<Next<{responseClassName}>> {SafeVariableName(requestClassName, responseClassName)};"
                    );
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
                    sourceBuilder.AppendLine
                    (@$"
                        {SafeVariableName(requestClassName)} = new Lazy<NextAsync>(BuildAsync<{requestClassName}>);"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                        {SafeVariableName(requestClassName)} = new Lazy<Next>(Build<{requestClassName}>);"
                    );
                }
            }

            foreach (var (requestClassName, responseClassName, isAsync) in irequestsOfT)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendLine
                    (@$"
                        {SafeVariableName(requestClassName, responseClassName)} = new Lazy<NextAsync<{responseClassName}>>(BuildAsync<{requestClassName},{responseClassName}>);"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                       {SafeVariableName(requestClassName, responseClassName)} = new Lazy<Next<{responseClassName}>>(Build<{requestClassName},{responseClassName}>);"
                    );
                }
            }
        }


        public static void LazyFactoryMethods(ref StringBuilder sourceBuilder)
        {

            sourceBuilder.AppendLine
            (@$"
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
            (@$"
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
            (@$"
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
            (@$"
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
