using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var items = source.ToArray();

            return items
                .SelectMany(c => selector(c).Flatten(selector))
                .Concat(items);
        }
    }
}
