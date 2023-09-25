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
        public void AddItem<TValue>(string key, TValue? value)
        {
            _items.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue? GetValue<TValue>(string key)
        {
            return (TValue?)_items[key];
        }

        public bool TryGetValue<TValue>(string key, out TValue? value)
        {
            return _items.TryGetValue(key, out value);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct PipelineContext
    {
        private FrugalDictionary _items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddItem(string key, object? value)
        {
            _items.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? GetValue(string key)
        {
            return _items[key];
        }

        public bool TryGetValue(string key, out object? value)
        {
            return _items.TryGetValue(key, out value);
        }

    }

}