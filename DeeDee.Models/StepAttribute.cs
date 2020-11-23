using System;

namespace DeeDee.Models
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
