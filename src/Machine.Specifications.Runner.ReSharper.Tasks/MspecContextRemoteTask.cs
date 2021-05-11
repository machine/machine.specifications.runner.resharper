using System;

namespace Machine.Specifications.Runner.ReSharper.Tasks
{
    [Serializable]
    public class MspecContextRemoteTask : MspecRemoteTask
    {
        public MspecContextRemoteTask(string typeName, string subject, string[] tags)
            : base(typeName)
        {
            Subject = subject;
            Tags = tags;
        }

        public MspecContextRemoteTask(string testId, bool runAllChildren, bool runExplicitly)
            : base(testId, runAllChildren, runExplicitly)
        {
        }

        public string ContextTypeName => TestId;

        public string Subject { get; set; }

        public string[] Tags { get; set; }

        public static MspecContextRemoteTask ToClient(string testId, bool runAllChildren, bool runExplicitly)
        {
            return new(testId, runAllChildren, runExplicitly);
        }

        public static MspecContextRemoteTask ToServer(string typeName, string subject, string[] tags)
        {
            return new(typeName, subject, tags);
        }
    }
}
