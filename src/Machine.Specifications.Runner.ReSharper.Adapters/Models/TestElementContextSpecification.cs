using System.Linq;
using System.Reflection;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Models
{
    public class TestElementContextSpecification : IContextSpecification
    {
        public TestElementContextSpecification(Specification specification)
        {
            Context = new TestElementContext(specification.Context);
            ContainingType = specification.ContainingType;
            FieldName = specification.FieldName;
            IsBehavior = specification.ContainingType != specification.Context.TypeName;
            Name = specification.Name;
            ParentFieldName = GetParentFieldName(specification);
        }

        public IContext Context { get; }

        public string ContainingType { get; }

        public string FieldName { get; }

        public string? ParentFieldName { get; }

        public bool IsBehavior { get; }

        public string Name { get; }

        private string? GetParentFieldName(Specification specification)
        {
            var type = specification.Assembly.GetType(Context.TypeName);

            var field = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.FieldType.IsGenericType)
                .Where(IsBehavesLikeField)
                .Where(x => x.FieldType.GenericTypeArguments.First().FullName == specification.ContainingType)
                .ToArray();

            if (field.Any())
            {
                return field.First().Name;
            }

            return null;
        }

        private bool IsBehavesLikeField(FieldInfo field)
        {
            return field.FieldType.GetCustomAttributes(false)
                .Any(x => x.GetType().FullName == FullNames.BehaviorDelegateAttribute);
        }
    }
}
