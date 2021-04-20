namespace DeeDee.Builders.Models
{
    internal static class IRequestBuilder
    {
        public static string Build(string ns)
        {
            var usings = $"namespace {ns}DeeDee.Models";

            return usings + @"
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
