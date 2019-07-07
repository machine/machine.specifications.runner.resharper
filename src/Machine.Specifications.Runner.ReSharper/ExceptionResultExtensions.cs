using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper
{
    public static class ExceptionResultExtensions
    {
        public static IEnumerable<ExceptionResult> Flatten(this ExceptionResult result)
        {
            var exception = result;

            if (exception.FullTypeName == typeof(TargetInvocationException).FullName && exception.InnerExceptionResult != null)
                exception = exception.InnerExceptionResult;

            for (var current = exception; current != null; current = current.InnerExceptionResult)
                yield return current;
        }
    }
}
