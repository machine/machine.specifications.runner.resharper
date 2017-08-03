using System;
using System.Collections.Generic;

namespace Machine.Specifications.ReSharperRunner
{
    public static class EnumerableExtensions
    {
        private static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
        {
            foreach (var s in source)
            {
                yield return s;

                var childs = childSelector(s);

                foreach (var c in childs.Flatten(childSelector))
                    yield return c;
            }
        }

        public static IEnumerable<T> Flatten<T>(this T source, Func<T, IEnumerable<T>> childSelector)
        {
            yield return source;

            var childs = childSelector(source);

            foreach (var c in childs.Flatten(childSelector))
                yield return c;
        }
    }
}