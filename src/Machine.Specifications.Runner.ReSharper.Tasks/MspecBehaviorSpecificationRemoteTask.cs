using System;

namespace Machine.Specifications.Runner.ReSharper.Tasks
{
    [Serializable]
    public class MspecBehaviorSpecificationRemoteTask : MspecRemoteTask
    {
        private MspecBehaviorSpecificationRemoteTask(string testId, string? ignoreReason, bool runAllChildren, bool runExplicitly)
            : base(testId, ignoreReason, runAllChildren, runExplicitly)
        {
        }

        private MspecBehaviorSpecificationRemoteTask(string parentId, string contextTypeName, string fieldName, string? ignoreReason)
            : base($"{contextTypeName}.{fieldName}", ignoreReason)
        {
            ParentId = parentId;
            ContextTypeName = contextTypeName;
            FieldName = fieldName;
        }

        public string? ParentId { get; set; }

        public string? ContextTypeName { get; set; }

        public string? FieldName { get; set; }

        public static MspecBehaviorSpecificationRemoteTask ToClient(string testId, string? ignoreReason, bool runAllChildren, bool runExplicitly)
        {
            return new(testId, ignoreReason, runAllChildren, runExplicitly);
        }

        public static MspecBehaviorSpecificationRemoteTask ToServer(string parentId, string contextTypeName, string fieldName, string? ignoreReason)
        {
            return new(parentId, contextTypeName, fieldName, ignoreReason);
        }
    }
}
