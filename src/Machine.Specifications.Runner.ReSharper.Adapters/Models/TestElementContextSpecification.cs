using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

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
        }

        public IContext Context { get; }

        public string ContainingType { get; }

        public string FieldName { get; }

        public bool IsBehavior { get; }

        public string Name { get; }
    }
}
