using System;

namespace Machine.Specifications.Runner.ReSharper.Tasks
{
    [Serializable]
    public class MspecContextSpecificationRemoteTask : MspecRemoteTask
    {
        public MspecContextSpecificationRemoteTask(string testId, bool runAllChildren, bool runExplicitly)
            : base(testId, runAllChildren, runExplicitly)
        {
        }

        public MspecContextSpecificationRemoteTask(
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

        public static MspecContextSpecificationRemoteTask ToClient(string testId, bool runAllChildren, bool runExplicitly)
        {
            return new MspecContextSpecificationRemoteTask(testId, runAllChildren, runExplicitly);
        }

        public static MspecContextSpecificationRemoteTask ToServer(
            string contextTypeName,
            string specificationFieldName,
            string displayName,
            string subject,
            string[] tags)
        {
            return new MspecContextSpecificationRemoteTask(contextTypeName, specificationFieldName, displayName, subject, tags);
        }
    }
}
