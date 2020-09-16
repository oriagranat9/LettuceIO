using System;
using System.Collections.Generic;
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

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable) => enumerable.Shuffle(new Random());

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, Random rng) =>
            enumerable.OrderBy(arg => rng.Next());
    }
}