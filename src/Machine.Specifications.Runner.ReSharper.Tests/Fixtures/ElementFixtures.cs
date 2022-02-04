using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Tests.Fixtures
{
    public static class ElementFixtures
    {
        public static ContextElement Context { get; } = new("Namespace.Context", string.Empty);

        public static BehaviorElement Behavior { get; } = new(Context, "Namespace.ABehavior", "behaves_like");

        public static SpecificationElement Specification { get; } = new(Context, "should");

        public static SpecificationElement BehaviorSpecification { get; } = new(Context, "should_behave", Behavior);

        public static SpecificationElement SecondBehaviorSpecification { get; } = new(Context, "should_not_behave", Behavior);
    }
}
