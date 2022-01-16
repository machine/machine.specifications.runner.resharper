using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public static class SpecificationInfoExtensions
    {
        public static ISpecificationElement ToElement(this SpecificationInfo specification, IContextElement context, ISpecificationElement? behavior = null)
        {
            return new SpecificationElement(context, specification.ContainingType, specification.FieldName, specification.Name, behavior);
        }

        public static bool IsBehavior(this SpecificationInfo specification, string contextTypeName)
        {
            return specification.ContainingType != contextTypeName;
        }
    }
}
