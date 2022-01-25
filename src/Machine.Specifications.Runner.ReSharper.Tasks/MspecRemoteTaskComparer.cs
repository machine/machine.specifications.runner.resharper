using System.Collections.Generic;

namespace Machine.Specifications.Runner.ReSharper.Tasks
{
    public class MspecRemoteTaskComparer : IEqualityComparer<MspecRemoteTask>
    {
        public static readonly MspecRemoteTaskComparer Default = new();

        public bool Equals(MspecRemoteTask x, MspecRemoteTask y)
        {
            return x.TestId == y.TestId;
        }

        public int GetHashCode(MspecRemoteTask obj)
        {
            return obj.TestId.GetHashCode();
        }
    }
}
