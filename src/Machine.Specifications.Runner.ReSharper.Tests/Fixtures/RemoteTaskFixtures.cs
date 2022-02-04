using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Tests.Fixtures
{
    public static class RemoteTaskFixtures
    {
        public static MspecContextRemoteTask Context { get; } = MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);

        public static MspecSpecificationRemoteTask Specification { get; } = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should", null, null, null, null);

        public static MspecSpecificationRemoteTask Behavior { get; } = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "behaves_like", "Namespace.ABehavior", null, null, null);

        public static MspecBehaviorSpecificationRemoteTask BehaviorSpecification { get; } = MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "should_behave", null);

        public static MspecBehaviorSpecificationRemoteTask SecondBehaviorSpecification { get; } = MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "should_not_behave", null);
    }
}
