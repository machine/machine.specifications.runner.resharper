using System;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tasks
{
    [Serializable]
    public class MspecTestSpecificationRemoteTask : MspecElementRemoteTask
    {
        public MspecTestSpecificationRemoteTask(string testId, bool runAllChildren, bool runExplicitly)
            : base(testId, runAllChildren, runExplicitly)
        {
        }

        public MspecTestSpecificationRemoteTask(
            string parentId,
            string contextTypeName,
            string specificationFieldName,
            string displayName,
            string subject,
            string[] tags)
            : base(parentId + "." + specificationFieldName)
        {
            ParentId = parentId;
            ContextTypeName = contextTypeName;
            SpecificationFieldName = specificationFieldName;
            DisplayName = displayName;
            Subject = subject;
            Tags = tags;
        }

        public string ParentId { get; }

        public string ContextTypeName { get; }

        public string SpecificationFieldName { get; }

        public string DisplayName { get; }

        public string Subject { get; }

        public string[] Tags { get; set; }

        public static MspecTestSpecificationRemoteTask ToClient(string testId, bool runAllChildren, bool runExplicitly)
        {
            return new MspecTestSpecificationRemoteTask(testId, runAllChildren, runExplicitly);
        }

        public static MspecTestSpecificationRemoteTask ToServer(
            string parentId,
            string contextTypeName,
            string specificationFieldName,
            string displayName,
            string subject,
            string[] tags)
        {
            return new MspecTestSpecificationRemoteTask(parentId, contextTypeName, specificationFieldName, displayName, subject, tags);
        }
    }
}
