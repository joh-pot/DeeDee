namespace DeeDee.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class StepAttribute : Attribute
    {
        public int Order { get; }

        public StepAttribute(int order)
        {
            Order = order;
        }
    }
}