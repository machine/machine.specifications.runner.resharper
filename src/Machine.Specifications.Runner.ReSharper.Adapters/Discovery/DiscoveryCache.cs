using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    public class DiscoveryCache
    {
        private readonly Assembly assembly;

        private readonly Dictionary<string, Type?> types = new();

        private readonly Dictionary<string, FieldInfo?> fields = new();

        private readonly Dictionary<string, FieldInfo?> behaviorFieldsByType = new();

        private readonly Dictionary<string, string?> ignoreReasons = new();

        private readonly Dictionary<string, IBehaviorElement> behaviors = new();

        public DiscoveryCache(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public IBehaviorElement? GetOrAddBehavior(IContextElement context, SpecificationInfo specification)
        {
            var behaviorField = GetOrAddBehaviorFieldByType(context.TypeName, specification.ContainingType);

            if (behaviorField == null)
            {
                return null;
            }

            var key = $"{context.TypeName}.{behaviorField.Name}";

            if (!behaviors.TryGetValue(key, out var behavior))
            {
                // MSpec behaviors don't seem to inherit an 'ignored' attribute from the context
                var ignoreReason = GetIgnoreReason(context.TypeName, behaviorField.Name, false);

                behavior = behaviors[key] = new BehaviorElement(context, specification.ContainingType, behaviorField.Name, ignoreReason);
            }

            return behavior;
        }

        public string? GetIgnoreReason(string typeName)
        {
            if (!ignoreReasons.TryGetValue(typeName, out var reason))
            {
                var type = GetOrAddType(typeName);

                reason = type == null
                    ? null
                    : GetIgnoreReason(type);

                ignoreReasons[typeName] = reason;
            }

            return reason;
        }

        public string? GetIgnoreReason(string typeName, string fieldName, bool inherit)
        {
            var key = $"{typeName}.{fieldName}";

            if (!ignoreReasons.TryGetValue(key, out var reason))
            {
                var type = GetOrAddType(typeName);
                var field = GetOrAddField(type, fieldName);

                reason = field == null
                    ? null
                    : GetIgnoreReason(field);

                if (string.IsNullOrEmpty(reason) && inherit)
                {
                    reason = GetIgnoreReason(typeName);
                }

                ignoreReasons[key] = reason;
            }

            return reason;
        }

        private FieldInfo? GetOrAddBehaviorFieldByType(string contextTypeName, string behaviorType)
        {
            var key = $"{contextTypeName}.{behaviorType}";

            if (!behaviorFieldsByType.TryGetValue(key, out var field))
            {
                var type = GetOrAddType(contextTypeName);

                field = GetBehaviorField(type, x => x.FieldType.GenericTypeArguments.First().FullName == behaviorType);

                behaviorFieldsByType[key] = field;

                if (field != null)
                {
                    fields[$"{contextTypeName}.{field.Name}"] = field;
                }
            }

            return field;
        }

        private string? GetIgnoreReason(MemberInfo member)
        {
            var attribute = member.GetCustomAttributesData()
                .FirstOrDefault(x => x.AttributeType.FullName == FullNames.IgnoreAttribute);

            var attributeValue = attribute?.ConstructorArguments.FirstOrDefault();

            return attributeValue?.Value as string;
        }

        private Type? GetOrAddType(string typeName)
        {
            if (!types.TryGetValue(typeName, out var type))
            {
                type = types[typeName] = assembly.GetType(typeName);
            }

            return type;
        }

        private FieldInfo? GetOrAddField(Type? type, string fieldName, Func<FieldInfo, bool>? predicate = null)
        {
            if (type == null)
            {
                return null;
            }

            var key = $"{type.FullName}.{fieldName}";

            if (!fields.TryGetValue(key, out var field))
            {
                field = type
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.Name == fieldName)
                    .FirstOrDefault(predicate ?? (_ => true));

                fields[key] = field;
            }

            return field;
        }

        private FieldInfo? GetBehaviorField(Type? type, Func<FieldInfo, bool> predicate)
        {
            return type?
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(IsBehavesLikeField)
                .FirstOrDefault(predicate);
        }

        private bool IsBehavesLikeField(FieldInfo field)
        {
            return field.FieldType.IsGenericType &&
                   field.FieldType.GetCustomAttributes(false)
                       .Any(x => x.GetType().FullName == FullNames.BehaviorDelegateAttribute);
        }
    }
}
