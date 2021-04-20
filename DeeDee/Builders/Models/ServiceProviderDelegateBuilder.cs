namespace DeeDee.Builders.Models
{
    internal static class ServiceProviderDelegateBuilder
    {
        public static string Build(string ns)
        {
            var usings = $@"
using System;
using System.Collections.Generic;

namespace {ns}DeeDee.Models
";

            return usings + @"
{
    public delegate object ServiceProvider(Type type);

    public static class ServiceProviderExtensions
    {
        public static IEnumerable<T> GetServices<T>(this ServiceProvider factory)
            => (IEnumerable<T>)factory(typeof(IEnumerable<T>));
    }

}

";
        }
    }
}
