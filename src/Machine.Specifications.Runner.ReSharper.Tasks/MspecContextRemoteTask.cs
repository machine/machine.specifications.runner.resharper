using System;

namespace Machine.Specifications.Runner.ReSharper.Tasks
{
    [Serializable]
    public class MspecContextRemoteTask : MspecRemoteTask
    {
        public MspecContextRemoteTask(string testId, string? ignoreReason, bool runAllChildren, bool runExplicitly)
            : base(testId, ignoreReason, runAllChildren, runExplicitly)
        {
        }

        public MspecContextRemoteTask(string typeName, string? subject, string[]? tags, string? ignoreReason)
            : base(typeName, ignoreReason)
        {
            Subject = subject;
            Tags = tags;
        }

        public string ContextTypeName => TestId;

        public string? Subject { get; set; }

        public string[]? Tags { get; set; }

        public static MspecContextRemoteTask ToClient(string testId, string? ignoreReason, bool runAllChildren, bool runExplicitly)
        {
            return new(testId, ignoreReason, runAllChildren, runExplicitly);
        }

        public static MspecContextRemoteTask ToServer(string typeName, string? subject, string[]? tags, string? ignoreReason)
        {
            return new(typeName, subject, tags, ignoreReason);
        }
    }
}
