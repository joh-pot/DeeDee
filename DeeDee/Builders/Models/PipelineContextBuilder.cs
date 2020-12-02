namespace DeeDee.Builders.Models
{
    internal static class PipelineContextBuilder
    {
        public static string Build()
        {
            return @"
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#nullable enable
namespace DeeDee.Models
{
    [StructLayout(LayoutKind.Auto)]
    public struct PipelineContext<TResponse>
    {
        public TResponse Result { get; set; }

        private FrugalDictionary _items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddItem(object key, object value)
        {
            _items.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetValue(object key)
        {
            return _items[key];
        }

        public bool TryGetValue(object key, out object? value)
        {
            return _items.TryGetValue(key, out value);
        }

    }

    [StructLayout(LayoutKind.Auto)]
    public struct PipelineContext
    {
        private FrugalDictionary _items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddItem(object key, object value)
        {
            _items.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetValue(object key)
        {
            return _items[key];
        }

        public bool TryGetValue(object key, out object? value)
        {
            return _items.TryGetValue(key, out value);
        }

    }

}

";
        }
    }
}
