using System;

namespace Machine.Specifications.Runner.ReSharper.Tasks
{
    [Serializable]
    public class MspecBehaviorSpecificationRemoteTask : MspecRemoteTask
    {
        public MspecBehaviorSpecificationRemoteTask(string testId, string ignoreReason, bool runAllChildren, bool runExplicitly)
            : base(testId, ignoreReason, runAllChildren, runExplicitly)
        {
        }

        public MspecBehaviorSpecificationRemoteTask(string contextTypeName, string behaviorFieldName, string specificationFieldName, string ignoreReason)
            : base($"{contextTypeName}::{specificationFieldName}", ignoreReason)
        {
            ContextTypeName = contextTypeName;
            BehaviorFieldName = behaviorFieldName;
            SpecificationFieldName = specificationFieldName;
        }

        public string ContextTypeName { get; set; }

        public string BehaviorFieldName { get; set; }

        public string SpecificationFieldName { get; set; }

        public static MspecBehaviorSpecificationRemoteTask ToClient(string testId, string ignoreReason, bool runAllChildren, bool runExplicitly)
        {
            return new(testId, ignoreReason, runAllChildren, runExplicitly);
        }

        public static MspecBehaviorSpecificationRemoteTask ToServer(string contextTypeName, string behaviorFieldName, string specificationFieldName, string ignoreReason)
        {
            return new(contextTypeName, behaviorFieldName, specificationFieldName, ignoreReason);
        }
    }
}
