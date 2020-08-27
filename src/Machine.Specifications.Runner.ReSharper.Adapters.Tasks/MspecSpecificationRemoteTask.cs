using System;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tasks
{
    [Serializable]
    public class MspecSpecificationRemoteTask : MspecRemoteTask
    {
        public MspecSpecificationRemoteTask(string testId, bool runAllChildren, bool runExplicitly)
            : base(testId, runAllChildren, runExplicitly)
        {
        }

        public MspecSpecificationRemoteTask(
            string contextTypeName,
            string specificationFieldName,
            string displayName,
            string subject,
            string[] tags)
            : base(contextTypeName + "::" + specificationFieldName)
        {
            ContextTypeName = contextTypeName;
            SpecificationFieldName = specificationFieldName;
            DisplayName = displayName;
            Subject = subject;
            Tags = tags;
        }

        public string ContextTypeName { get; set; }

        public string SpecificationFieldName { get; set; }

        public string DisplayName { get; }

        public string Subject { get; }

        public string[] Tags { get; set; }

        public static MspecSpecificationRemoteTask ToClient(string testId, bool runAllChildren, bool runExplicitly)
        {
            return new MspecSpecificationRemoteTask(testId, runAllChildren, runExplicitly);
        }

        public static MspecSpecificationRemoteTask ToServer(
            string contextTypeName,
            string specificationFieldName,
            string displayName,
            string subject,
            string[] tags)
        {
            return new MspecSpecificationRemoteTask(contextTypeName, specificationFieldName, displayName, subject, tags);
        }
    }
}
