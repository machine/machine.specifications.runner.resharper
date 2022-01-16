using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    public class DiscoveryCache
    {
        private readonly Dictionary<string, string?> behaviorFields = new();

        private readonly Dictionary<string, ISpecificationElement> behaviors = new();

        public string? GetOrAddBehaviorField(Assembly assembly, string contextTypeName, string behaviorType)
        {
            var key = $"{contextTypeName}.{behaviorType}";

            if (!behaviorFields.TryGetValue(key, out var field))
            {
                var type = assembly.GetType(contextTypeName);

                var value = type
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.FieldType.IsGenericType)
                    .Where(IsBehavesLikeField)
                    .FirstOrDefault(x => x.FieldType.GenericTypeArguments.First().FullName == behaviorType);

                field = behaviorFields[key] = value?.Name;
            }

            return field;
        }

        public ISpecificationElement? GetOrAddBehavior(Assembly assembly, IContextElement context, SpecificationInfo specification)
        {
            var behaviorField = GetOrAddBehaviorField(assembly, context.TypeName, specification.ContainingType);

            if (behaviorField == null)
            {
                return null;
            }

            var key = $"{context.TypeName}.{behaviorField}";

            if (!behaviors.TryGetValue(key, out var behavior))
            {
                behavior = behaviors[key] = new SpecificationElement(context, context.TypeName, behaviorField, behaviorField.ToFormat());
            }

            return behavior;
        }

        private bool IsBehavesLikeField(FieldInfo field)
        {
            return field.FieldType.GetCustomAttributes(false)
                .Any(x => x.GetType().FullName == FullNames.BehaviorDelegateAttribute);
        }
    }
}
