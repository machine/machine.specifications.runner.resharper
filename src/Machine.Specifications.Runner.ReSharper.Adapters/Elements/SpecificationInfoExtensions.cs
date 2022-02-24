using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public static class SpecificationInfoExtensions
    {
        public static ISpecificationElement ToElement(this SpecificationInfo specification, IContextElement context, string? ignoreReason, IBehaviorElement? behavior = null)
        {
            return new SpecificationElement(context, specification.FieldName, ignoreReason, behavior);
        }

        public static bool IsBehavior(this SpecificationInfo specification, string contextTypeName)
        {
            return specification.ContainingType != contextTypeName;
        }
    }
}
