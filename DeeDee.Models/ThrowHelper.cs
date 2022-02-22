namespace DeeDee.Models
{
    internal static class ThrowHelper
    {
        //[DoesNotReturn]
        public static void ThrowKeyNotFound()
        {
            throw new KeyNotFoundException();
        }
    }
}