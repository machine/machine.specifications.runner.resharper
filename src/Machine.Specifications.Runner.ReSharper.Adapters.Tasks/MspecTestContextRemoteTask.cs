using System;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tasks
{
    [Serializable]
    public class MspecTestContextRemoteTask : MspecElementRemoteTask
    {
        public MspecTestContextRemoteTask(string typeName)
            : base(typeName)
        {
        }

        public MspecTestContextRemoteTask(string testId, bool runAllChildren, bool runExplicitly)
            : base(testId, runAllChildren, runExplicitly)
        {
        }

        public string TypeName => TestId;

        public static MspecTestContextRemoteTask ToClient(string testId, bool runAllChildren, bool runExplicitly)
        {
            return new MspecTestContextRemoteTask(testId, runAllChildren, runExplicitly);
        }

        public static MspecTestContextRemoteTask ToServer(string typeName)
        {
            return new MspecTestContextRemoteTask(typeName);
        }
    }
}
