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
            return type.GetCustomAttributes(FullNames.TagsAttribute)
                .SelectMany(x => x.GetParameters());
        }

        public static string GetSubject(this ITypeInfo type)
        {
            return type.GetCustomAttributes(FullNames.SubjectAttribute)
                .FirstOrDefault()?
                .GetParameters()
                .Join(" ");
        }

        public static bool IsSpecification(this IFieldInfo field)
        {
            return field.FieldType.GetCustomAttributes(FullNames.AssertDelegateAttribute).Any();
        }

        public static bool IsBehavior(this IFieldInfo field)
        {
            var behaviorType = field.FieldType.GetGenericArguments().FirstOrDefault();

            if (behaviorType == null)
                return false;

            return field.FieldType.GetCustomAttributes(FullNames.BehaviorDelegateAttribute).Any() &&
                   behaviorType.GetCustomAttributes(FullNames.BehaviorsAttribute).Any();
        }

        public static bool IsIgnored(this ITypeInfo type)
        {
            return type.GetCustomAttributes(FullNames.IgnoreAttribute).Any();
        }

        public static bool IsIgnored(this IFieldInfo field)
        {
            return field.GetCustomAttributes(FullNames.IgnoreAttribute).Any();
        }

        public static bool IsBehaviorContainer(this ITypeInfo type)
        {
            return !type.IsAbstract &&
                   !type.GetGenericArguments().Any() &&
                   type.GetCustomAttributes(FullNames.BehaviorsAttribute).Any() &&
                   type.GetFields().Any(x => x.IsSpecification());
        }

        public static bool IsContext(this ITypeInfo type)
        {
            var fields = type.GetFields();

            return !type.IsAbstract &&
                   !type.GetCustomAttributes(FullNames.BehaviorsAttribute).Any() &&
                   fields.Any(x => x.IsSpecification() || x.IsBehavior());
        }
    }
}
