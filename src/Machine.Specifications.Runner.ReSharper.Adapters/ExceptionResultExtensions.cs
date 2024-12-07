using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters;

internal static class ExceptionResultExtensions
{
    private static IEnumerable<ExceptionResult> Flatten(this ExceptionResult result)
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

    public static ExceptionInfo[] GetExceptions(this ExceptionResult? result)
    {
        if (result == null)
        {
            return [];
        }

        return result.Flatten()
            .Select(x => new ExceptionInfo(x.FullTypeName, x.Message, x.StackTrace))
            .ToArray();
    }

    public static string GetExceptionMessage(this ExceptionResult? result)
    {
        if (result == null)
        {
            return string.Empty;
        }

        var exception = result.Flatten().FirstOrDefault();

        return exception != null
            ? $"{exception.FullTypeName}: {exception.Message}"
            : string.Empty;
    }
}
