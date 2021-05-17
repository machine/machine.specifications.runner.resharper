using System;

namespace Machine.Specifications.Runner.ReSharper.Tasks
{
    [Serializable]
    public class MspecBehaviorRemoteTask : MspecRemoteTask
    {
        public MspecBehaviorRemoteTask(string testId, string ignoreReason, bool runAllChildren, bool runExplicitly)
            : base(testId, ignoreReason, runAllChildren, runExplicitly)
        {
        }

        public MspecBehaviorRemoteTask(string contextTypeName, string behaviorFieldName, string ignoreReason)
            : base($"{contextTypeName}::{behaviorFieldName}", ignoreReason)
        {
            ContextTypeName = contextTypeName;
            BehaviorFieldName = behaviorFieldName;
        }

        public string ContextTypeName { get; set; }

        public string BehaviorFieldName { get; set; }

        public static MspecBehaviorRemoteTask ToClient(string testId, string ignoreReason, bool runAllChildren, bool runExplicitly)
        {
            return new(testId, ignoreReason, runAllChildren, runExplicitly);
        }

        public static MspecBehaviorRemoteTask ToServer(string contextTypeName, string behaviorFieldName, string ignoreReason)
        {
            return new(contextTypeName, behaviorFieldName, ignoreReason);
        }
    }
}
