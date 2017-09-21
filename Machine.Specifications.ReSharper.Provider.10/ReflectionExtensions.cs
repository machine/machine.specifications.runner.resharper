using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider.Reflection;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<string> GetTags(this ITypeInfo type)
        {
            throw new NotImplementedException();
        }

        public static string GetSubject(this ITypeInfo type)
        {
            throw new NotImplementedException();
        }

        public static bool IsSpecification(this IFieldInfo field)
        {
            return field.FieldType.GetCustomAttributes(FullNames.AssertDelegateAttribute, false).Any();
        }

        public static bool IsBehavior(this IFieldInfo field)
        {
            var fieldType = field.FieldType;
            var arguments = fieldType.GetGenericArguments();

            return fieldType.GetCustomAttributes(FullNames.BehaviorDelegateAttribute, false).Any() &&
                   arguments.Any(x => x.GetCustomAttributes(FullNames.BehaviorsAttribute, false).Any());
        }

        public static bool IsIgnored(this ITypeInfo type)
        {
            throw new NotImplementedException();
        }

        public static bool IsIgnored(this IFieldInfo field)
        {
            throw new NotImplementedException();
        }

        public static bool IsBehaviorContainer(this ITypeInfo type)
        {
            throw new NotImplementedException();
        }

        public static bool IsContext(this ITypeInfo type)
        {
            throw new NotImplementedException();
        }
    }
}
