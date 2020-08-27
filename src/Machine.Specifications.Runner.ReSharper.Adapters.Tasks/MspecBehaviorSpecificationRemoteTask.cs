using System;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tasks
{
    [Serializable]
    public class MspecBehaviorSpecificationRemoteTask : MspecRemoteTask
    {
        public MspecBehaviorSpecificationRemoteTask(string testId, bool runAllChildren, bool runExplicitly)
            : base(testId, runAllChildren, runExplicitly)
        {
        }

        public MspecBehaviorSpecificationRemoteTask(string contextTypeName, string behaviorFieldName, string specificationFieldName)
            : base(contextTypeName + "::" + specificationFieldName)
        {
            ContextTypeName = contextTypeName;
            BehaviorFieldName = behaviorFieldName;
            SpecificationFieldName = specificationFieldName;
        }

        public string ContextTypeName { get; set; }

        public string BehaviorFieldName { get; set; }

        public string SpecificationFieldName { get; set; }

        public static MspecBehaviorSpecificationRemoteTask ToClient(string testId, bool runAllChildren, bool runExplicitly)
        {
            return new MspecBehaviorSpecificationRemoteTask(testId, runAllChildren, runExplicitly);
        }

        public static MspecBehaviorSpecificationRemoteTask ToServer(
            string contextTypeName,
            string behaviorFieldName,
            string specificationFieldName)
        {
            return new MspecBehaviorSpecificationRemoteTask(contextTypeName, behaviorFieldName, specificationFieldName);
        }
    }
}
