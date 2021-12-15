using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Models
{
    public class FrameworkContextSpecification : IContextSpecification
    {
        public FrameworkContextSpecification(ContextInfo context, SpecificationInfo specification)
        {
            Context = new FrameworkContext(context);
            ContainingType = specification.ContainingType;
            FieldName = specification.FieldName;
            IsBehavior = context.TypeName != specification.ContainingType;
            Name = specification.Name;
        }

        public IContext Context { get; }

        public string ContainingType { get; }

        public string FieldName { get; }

        public string? ParentFieldName { get; }

        public bool IsBehavior { get; }

        public string Name { get; }
    }
}
