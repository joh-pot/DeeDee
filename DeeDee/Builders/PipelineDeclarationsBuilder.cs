using System.Collections.Generic;
using System.Text;

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
            foreach (var (RequestClassName, IsAsync) in irequests)
            {
                if (IsAsync)
                {
                    sourceBuilder.AppendLine
                    (@$"
                        private readonly Lazy<NextAsync> {SafeVariableName(RequestClassName)};"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                        private readonly Lazy<Next> {SafeVariableName(RequestClassName)};"
                    );
                }
            }

            foreach (var (RequestClassName, ResponseClassName, IsAsync) in irequestsOfT)
            {
                if (IsAsync)
                {
                    sourceBuilder.AppendLine
                    (@$"
                        private readonly Lazy<NextAsync<{ResponseClassName}>> {SafeVariableName(RequestClassName, ResponseClassName)};"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                        private readonly Lazy<Next<{ResponseClassName}>> {SafeVariableName(RequestClassName, ResponseClassName)};"
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
            foreach (var (RequestClassName, IsAsync) in irequests)
            {
                if (IsAsync)
                {
                    sourceBuilder.AppendLine
                    (@$"
                        {SafeVariableName(RequestClassName)} = new Lazy<NextAsync>(BuildAsync<{RequestClassName}>);"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                        {SafeVariableName(RequestClassName)} = new Lazy<Next>(Build<{RequestClassName}>);"
                    );
                }
            }

            foreach (var (RequestClassName, ResponseClassName, IsAsync) in irequestsOfT)
            {
                if (IsAsync)
                {
                    sourceBuilder.AppendLine
                    (@$"
                        {SafeVariableName(RequestClassName, ResponseClassName)} = new Lazy<NextAsync<{ResponseClassName}>>(BuildAsync<{RequestClassName},{ResponseClassName}>);"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    (@$"
                       {SafeVariableName(RequestClassName, ResponseClassName)} = new Lazy<Next<{ResponseClassName}>>(Build<{RequestClassName},{ResponseClassName}>);"
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


       
        public static string SafeVariableName(string requestClassName)
        {
            return $"_{requestClassName.Replace(".", "_")}_lazy";
        }

        public static string SafeVariableName(string requestClassName, string responseClassName)
        {
            return $"_{requestClassName.Replace(".", "_")}_{responseClassName.Replace(".", "_")}_lazy";
        }

    }
}
