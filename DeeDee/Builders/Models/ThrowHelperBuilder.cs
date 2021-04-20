namespace DeeDee.Builders.Models
{
    internal static class ThrowHelperBuilder
    {
        public static string Build(string ns)
        {
            var usings = $@"
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace {ns}DeeDee.Models
";

            return usings + @"
{
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        public static void ThrowKeyNotFound()
        {
            throw new KeyNotFoundException();
        }
    }
}
";
        }
    }
}
