namespace DeeDee.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BindAsAttribute : Attribute
    {
        public Lifetime Lifetime { get; }

        public BindAsAttribute(Lifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
