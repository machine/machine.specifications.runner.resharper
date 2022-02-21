using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    internal static class SpecificationInfoExtensions
    {
        public static TestSpecificationInfo ToTestSpecification(this SpecificationInfo specification)
        {
            return new TestSpecificationInfo(specification.ContainingType, specification.FieldName, specification.CapturedOutput);
        }
    }
}
