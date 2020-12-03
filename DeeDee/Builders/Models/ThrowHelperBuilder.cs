namespace DeeDee.Builders.Models
{
    internal static class ThrowHelperBuilder
    {
        public static string Build()
        {
            return @"
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DeeDee.Models
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
