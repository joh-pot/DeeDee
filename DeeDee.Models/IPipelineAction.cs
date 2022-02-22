namespace DeeDee.Models
{
    public interface IPipelineActionAsync<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> InvokeAsync
        (
            TRequest request,
            PipelineContext<TResponse> context,
            NextAsync<TResponse> next,
            CancellationToken cancellationToken = default
        );
    }

    public interface IPipelineActionAsync<in TRequest> where TRequest : IRequest
    {
        Task InvokeAsync
        (
            TRequest request,
            PipelineContext context,
            NextAsync next,
            CancellationToken cancellationToken = default
        );
    }

    public interface IPipelineAction<in TRequest> where TRequest : IRequest
    {
        void Invoke
        (
            TRequest request,
            ref PipelineContext context,
            Next next
        );
    }


    public interface IPipelineAction<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        TResponse Invoke
        (
            TRequest request,
            ref PipelineContext<TResponse> context,
            Next<TResponse> next
        );
    }
}