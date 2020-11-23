using System.Threading;
using System.Threading.Tasks;

namespace DeeDee.Models
{
    public delegate Task<TResponse> NextAsync<TResponse>
    (
        IRequest<TResponse> request,
        PipelineContext<TResponse> context,
        CancellationToken token = default
    );

    public delegate TResponse Next<TResponse>(IRequest<TResponse> request, ref PipelineContext<TResponse> context);

    public delegate Task NextAsync
    (
        IRequest request,
        PipelineContext context,
        CancellationToken token = default
    );

    public delegate void Next(IRequest request, ref PipelineContext context);
}
