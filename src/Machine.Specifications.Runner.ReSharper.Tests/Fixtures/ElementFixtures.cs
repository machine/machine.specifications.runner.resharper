using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Tests.Fixtures
{
    public static class ElementFixtures
    {
        public static ContextElement Context { get; } = new("Namespace.Context", string.Empty);

        public static BehaviorElement Behavior1 { get; } = new(Context, "Namespace.Behavior1", "behaves_like_1");

        public static BehaviorElement Behavior2 { get; } = new(Context, "Namespace.Behavior2", "behaves_like_2");

        public static SpecificationElement Specification1 { get; } = new(Context, "should_1");

        public static SpecificationElement Specification2 { get; } = new(Context, "should_2");

        public static SpecificationElement Behavior1Specification1 { get; } = new(Context, "should_behave_1", Behavior1);

        public static SpecificationElement Behavior1Specification2 { get; } = new(Context, "should_behave_2", Behavior1);

        public static SpecificationElement Behavior2Specification1 { get; } = new(Context, "should_behave_1", Behavior2);
    }
}
