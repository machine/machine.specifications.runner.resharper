using System.Collections.Generic;
using System.Reflection;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class ExceptionResultExtensions
    {
        public static IEnumerable<Utility.ExceptionResult> Flatten(this Utility.ExceptionResult result)
        {
            var exception = result;

            if (exception.FullTypeName == typeof(TargetInvocationException).FullName && exception.InnerExceptionResult != null)
            {
                exception = exception.InnerExceptionResult;
            }

            for (var current = exception; current != null; current = current.InnerExceptionResult)
            {
                yield return current;
            }
        }
    }
}
