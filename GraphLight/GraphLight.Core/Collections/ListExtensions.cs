using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Collections
{
    public static class ListExtensions
    {
        public static void Iter<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
                action(enumerator.Current);
        }

        public static void Iter<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var index = 0;
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var next = enumerator.Current;
                action(next, index++);
            }
        }

        public static void Iter<T>(this IEnumerable<T> enumerable, Action<T, T> action)
        {
            var enumerator = enumerable.GetEnumerator();
            enumerator.MoveNext();
            var prev = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var next = enumerator.Current;
                action(prev, next);
                prev = next;
            }
        }

        public static void CircleIter<T>(this IList<T> list, Action<T, T> action)
        {
            var cnt = list.Count;
            for (var i = 0; i < cnt; i++)
            {
                var j = (i + 1) % cnt;
                action(list[i], list[j]);
            }
        }

        public static void CircleIter<T>(this IList<T> list, Action<T, T, T> action)
        {
            var cnt = list.Count;
            for (var i = 0; i < cnt; i++)
            {
                var j = (i + 1) % cnt;
                var k = (i + 1) % cnt;
                action(list[i], list[j], list[k]);
            }
        }

        public static IDictionary<TKey, TVal> Backup<TKey, TVal>(this IEnumerable<TKey> enumerable, Func<TKey, TVal> map)
        {
            return enumerable.ToDictionary(x => x, map);
        }

        public static void Restore<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, Action<TKey, TVal> map)
        {
            dictionary.Iter(x => map(x.Key, x.Value));
        }
    }
}