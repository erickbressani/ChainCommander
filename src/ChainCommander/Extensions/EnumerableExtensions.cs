using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainCommander
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int howMany)
            => source.Skip(Math.Max(0, source.Count() - howMany));
    }
}