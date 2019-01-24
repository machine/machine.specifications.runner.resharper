﻿using System.Collections.Generic;
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
            return type.GetCustomAttributes(FullNames.TagsAttribute, false)
                .SelectMany(x => x.GetParameters())
                .Distinct();
        }

        public static string GetSubject(this ITypeInfo type)
        {
            var attributes = type.GetCustomAttributes(FullNames.SubjectAttribute, true)
                .ToArray();

            if (!attributes.Any())
                return type.GetContainingType()?.GetSubject() ?? string.Empty;

            return attributes.First()
                .GetParameters()
                .Join(" ");
        }

        public static bool IsSpecification(this IFieldInfo field)
        {
            return field.FieldType.GetCustomAttributes(FullNames.AssertDelegateAttribute, false).Any();
        }

        public static bool IsBehavior(this IFieldInfo field)
        {
            var type = field.FieldType;

            var arguments = type.GetGenericArguments()
                .SelectMany(x => x.GetCustomAttributes(FullNames.BehaviorsAttribute, false));

            return type.GetCustomAttributes(FullNames.BehaviorDelegateAttribute, false).Any() &&
                   arguments.Any();
        }

        public static bool IsIgnored(this ITypeInfo type)
        {
            return type.GetCustomAttributes(FullNames.IgnoreAttribute, false).Any();
        }

        public static bool IsIgnored(this IFieldInfo field)
        {
            return field.GetCustomAttributes(FullNames.IgnoreAttribute, false).Any();
        }

        public static bool IsBehaviorContainer(this ITypeInfo type)
        {
            return !type.IsAbstract &&
                   type.GetCustomAttributes(FullNames.BehaviorsAttribute, false).Any() &&
                   type.GetFields().Any(x => x.IsSpecification());
        }

        public static bool IsContext(this ITypeInfo type)
        {
            return !type.IsAbstract &&
                   !type.GetCustomAttributes(FullNames.BehaviorsAttribute, false).Any() &&
                   !type.GetGenericArguments().Any() &&
                   type.GetFields().Any(x => x.IsSpecification() || x.IsBehavior());
        }
    }
}
