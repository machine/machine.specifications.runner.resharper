using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tasks
{
    [Serializable]
    public abstract class MspecRemoteTask : RemoteTask
    {
        protected MspecRemoteTask(string testId, string ignoreReason, bool runAllChildren = true, bool runExplicitly = false)
        {
            TestId = testId;
            IgnoreReason = ignoreReason;
            RunAllChildren = runAllChildren;
            RunExplicitly = runExplicitly;
        }

        public string TestId { get; }

        public string IgnoreReason { get; }

        public bool RunAllChildren { get; }

        public bool RunExplicitly { get; }
    }
}
