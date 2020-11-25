namespace DeeDee.Builders.Models
{
    internal static class IRequestBuilder
    {
        public static string Build()
        {
            return @"
namespace DeeDee.Models
{ 
    public interface IRequest<TResponse>
    {
    }

    public interface IRequest
    {
    }
}

";
        }
    }
}
