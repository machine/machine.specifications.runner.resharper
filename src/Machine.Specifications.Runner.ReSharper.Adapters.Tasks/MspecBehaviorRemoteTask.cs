using System;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tasks
{
    [Serializable]
    public class MspecBehaviorRemoteTask : MspecRemoteTask
    {
        public MspecBehaviorRemoteTask(string testId, bool runAllChildren, bool runExplicitly)
            : base(testId, runAllChildren, runExplicitly)
        {
        }

        public MspecBehaviorRemoteTask(string contextTypeName, string behaviorFieldName, string specificationFieldName)
            : base(contextTypeName + "::" + behaviorFieldName)
        {
            ContextTypeName = contextTypeName;
            BehaviorFieldName = behaviorFieldName;
            SpecificationFieldName = specificationFieldName;
        }

        public string ContextTypeName { get; set; }

        public string BehaviorFieldName { get; set; }

        public string SpecificationFieldName { get; set; }

        public static MspecBehaviorRemoteTask ToClient(string testId, bool runAllChildren, bool runExplicitly)
        {
            return new MspecBehaviorRemoteTask(testId, runAllChildren, runExplicitly);
        }

        public static MspecBehaviorRemoteTask ToServer(
            string contextTypeName,
            string behaviorFieldName,
            string specificationFieldName)
        {
            return new MspecBehaviorRemoteTask(contextTypeName, behaviorFieldName, specificationFieldName);
        }
    }
}
