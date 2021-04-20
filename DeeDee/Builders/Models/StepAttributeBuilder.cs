namespace DeeDee.Builders.Models
{
    internal static class StepAttributeBuilder
    {
        public static string Build(string ns)
        {
            var usings = $@"
using System;

namespace {ns}DeeDee.Models
";

            return usings + @"
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class StepAttribute : Attribute
    {
        public int Order { get; }

        public StepAttribute(int order)
        {
            Order = order;
        }
    }
}

";
        }
    }
}
