using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LettuceIo.Dotnet.Base.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Loop<T>(this IEnumerable<T> enumerable)
        {
            var items = enumerable as T[] ?? enumerable.ToArray();
            while (true)
                foreach (var i in items)
                    yield return i;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, Random rng) =>
            enumerable.OrderBy(arg => rng.Next());

        public static IEnumerable<T[]> Split<T>(this IEnumerable<T> enumerable, int count) => enumerable
            .Select((v, i) => (v, i)).GroupBy(pair => pair.i % count)
            .Select(grouping => grouping.Select(pair => pair.v).ToArray());
    }
}