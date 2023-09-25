namespace DeeDee.Models
{
    internal static class ThrowHelper
    {
        public static void ThrowKeyNotFound()
        {
            throw new KeyNotFoundException();
        }
    }
}