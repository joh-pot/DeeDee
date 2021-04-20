namespace DeeDee.Builders.Models
{
    internal static class FrugalDictionaryBuilder
    {
        public static string Build(string ns)
        {
            var usings = $@"
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#nullable enable

namespace {ns}DeeDee.Models
";


            return usings + @"
{
    [StructLayout(LayoutKind.Auto)]
    internal struct FrugalDictionary
    {
        private KeyValuePair<object, object?> _one;
        private KeyValuePair<object, object?> _two;
        private KeyValuePair<object, object?> _three;
        private KeyValuePair<object, object?> _four;
        private KeyValuePair<object, object?> _five;
        private KeyValuePair<object, object?> _six;
        private KeyValuePair<object, object?> _seven;
        private KeyValuePair<object, object?> _eight;
        private KeyValuePair<object, object?> _nine;
        private KeyValuePair<object, object?> _ten;
        private byte _allocated;
        private Dictionary<object, object?>? _values;
      
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryCheck(ref KeyValuePair<object, object?> kvp, object key, out object? value)
        {
            if (kvp.Key.Equals(key))
            {
                value = kvp.Value;
                return true;
            }
            
            value = default;
            return false;
        }

        public bool TryGetValue(object key, out object? value)
        {
            var (found, val) = FindEntry(key);
            value = val;
            return found;
        }

        private static (bool Found, object? Value) Found(object? value) => (true, value);
        private static (bool Found, object? Value) NotFound() => (false, null);

        private (bool Found, object? Value) FindEntry(object key)
        {
            switch (_allocated)
            {
                case 0: return NotFound();
                case 1:
                {
                    return TryCheck(ref _one, key, out var val) ? Found(val) : NotFound();
                }
                case 2:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    return TryCheck(ref _two, key, out val) ? Found(val) : NotFound();
                }
                case 3:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    if (TryCheck(ref _two, key, out val))
                        return Found(val);
                    return TryCheck(ref _three, key, out val) ? Found(val) : NotFound();
                }
                case 4:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    if (TryCheck(ref _two, key, out val))
                        return Found(val);
                    if (TryCheck(ref _three, key, out val))
                        return Found(val);
                    return TryCheck(ref _four, key, out val) ? Found(val) : NotFound();
                }
                case 5:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    if (TryCheck(ref _two, key, out val))
                        return Found(val);
                    if (TryCheck(ref _three, key, out val))
                        return Found(val);
                    if (TryCheck(ref _four, key, out val))
                        return Found(val);
                    return TryCheck(ref _five, key, out val) ? Found(val) : NotFound();
                }
                case 6:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    if (TryCheck(ref _two, key, out val))
                        return Found(val);
                    if (TryCheck(ref _three, key, out val))
                        return Found(val);
                    if (TryCheck(ref _four, key, out val))
                        return Found(val);
                    if (TryCheck(ref _five, key, out val))
                        return Found(val);
                    return TryCheck(ref _six, key, out val) ? Found(val) : NotFound();
                }
                case 7:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    if (TryCheck(ref _two, key, out val))
                        return Found(val);
                    if (TryCheck(ref _three, key, out val))
                        return Found(val);
                    if (TryCheck(ref _four, key, out val))
                        return Found(val);
                    if (TryCheck(ref _five, key, out val))
                        return Found(val);
                    if (TryCheck(ref _six, key, out val))
                        return Found(val);
                    return TryCheck(ref _seven, key, out val) ? Found(val) : NotFound();
                }
                case 8:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    if (TryCheck(ref _two, key, out val))
                        return Found(val);
                    if (TryCheck(ref _three, key, out val))
                        return Found(val);
                    if (TryCheck(ref _four, key, out val))
                        return Found(val);
                    if (TryCheck(ref _five, key, out val))
                        return Found(val);
                    if (TryCheck(ref _six, key, out val))
                        return Found(val);
                    if (TryCheck(ref _seven, key, out val))
                        return Found(val);
                    return TryCheck(ref _eight, key, out val) ? Found(val) : NotFound();
                }
                case 9:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    if (TryCheck(ref _two, key, out val))
                        return Found(val);
                    if (TryCheck(ref _three, key, out val))
                        return Found(val);
                    if (TryCheck(ref _four, key, out val))
                        return Found(val);
                    if (TryCheck(ref _five, key, out val))
                        return Found(val);
                    if (TryCheck(ref _six, key, out val))
                        return Found(val);
                    if (TryCheck(ref _seven, key, out val))
                        return Found(val);
                    if (TryCheck(ref _eight, key, out val))
                        return Found(val);
                    return TryCheck(ref _nine, key, out val) ? Found(val) : NotFound();
                }
                case 10:
                {
                    if (TryCheck(ref _one, key, out var val))
                        return Found(val);
                    if (TryCheck(ref _two, key, out val))
                        return Found(val);
                    if (TryCheck(ref _three, key, out val))
                        return Found(val);
                    if (TryCheck(ref _four, key, out val))
                        return Found(val);
                    if (TryCheck(ref _five, key, out val))
                        return Found(val);
                    if (TryCheck(ref _six, key, out val))
                        return Found(val);
                    if (TryCheck(ref _seven, key, out val))
                        return Found(val);
                    if (TryCheck(ref _eight, key, out val))
                        return Found(val);
                    if (TryCheck(ref _nine, key, out val))
                        return Found(val);
                    return TryCheck(ref _ten, key, out val) ? Found(val) : NotFound();
                }
                default:
                {
                    return _values!.TryGetValue(key, out var val) ? Found(val) : NotFound();
                }
            }

        }

        public object? this[object key]
        {
            get
            {
                var entry = FindEntry(key);
                if (!entry.Found)
                    ThrowHelper.ThrowKeyNotFound();
                return entry.Value;
            }
        }

        public void Add(object key, object? value)
        {
            switch (_allocated)
            {

                case 0:
                    _one = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 1:
                    _two = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 2:
                    _three = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 3:
                    _four = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 4:
                    _five = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 5:
                    _six = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 6:
                    _seven = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 7:
                    _eight = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 8:
                    _nine = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                case 9:
                    _ten = new KeyValuePair<object, object?>(key, value);
                    ++_allocated;
                    return;
                default:
                {
                    _values ??= new Dictionary<object, object?>(11)
                    {
                        { _one.Key, _one.Value },
                        { _two.Key, _two.Value },
                        { _three.Key, _three.Value },
                        { _four.Key, _four.Value },
                        { _five.Key, _five.Value },
                        { _six.Key, _six.Value },
                        { _seven.Key, _seven.Value },
                        { _eight.Key, _eight.Value },
                        { _nine.Key, _nine.Value },
                        { _ten.Key, _ten.Value },
                    };
                    _values.Add(key, value);
                    _allocated = 11;
                    return;
                }
            }
        }

    }
}
";
        }
    }
}
