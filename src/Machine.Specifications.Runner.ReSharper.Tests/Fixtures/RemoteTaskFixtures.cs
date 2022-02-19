using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Tests.Fixtures
{
    public static class RemoteTaskFixtures
    {
        public static MspecContextRemoteTask Context { get; } =
            MspecContextRemoteTask.ToServer(
                ElementFixtures.Context.TypeName,
                null,
                null,
                null);

        public static MspecSpecificationRemoteTask Specification1 { get; } =
            MspecSpecificationRemoteTask.ToServer(
                ElementFixtures.Context.TypeName,
                ElementFixtures.Specification1.FieldName,
                null,
                null,
                null,
                null);

        public static MspecSpecificationRemoteTask Specification2 { get; } =
            MspecSpecificationRemoteTask.ToServer(
                ElementFixtures.Context.TypeName,
                ElementFixtures.Specification2.FieldName,
                null,
                null,
                null,
                null);

        public static MspecSpecificationRemoteTask Behavior1 { get; } =
            MspecSpecificationRemoteTask.ToServer(
                ElementFixtures.Context.TypeName,
                ElementFixtures.Behavior1.FieldName,
                ElementFixtures.Behavior1.TypeName,
                null,
                null,
                null);

        public static MspecBehaviorSpecificationRemoteTask Behavior1Specification1 { get; } =
            MspecBehaviorSpecificationRemoteTask.ToServer(
                ElementFixtures.Behavior1.Id,
                ElementFixtures.Context.TypeName,
                ElementFixtures.Behavior1Specification1.FieldName,
                null);

        public static MspecBehaviorSpecificationRemoteTask Behavior1Specification2 { get; } =
            MspecBehaviorSpecificationRemoteTask.ToServer(
                ElementFixtures.Behavior1.Id,
                ElementFixtures.Context.TypeName,
                ElementFixtures.Behavior1Specification2.FieldName,
                null);
    }
}
