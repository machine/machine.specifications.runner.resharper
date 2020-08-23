using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tasks
{
    [Serializable]
    public class MspecAssemblyRemoteTask : TestContainer
    {
        public MspecAssemblyRemoteTask(string location, ShadowCopy shadowCopy)
            : base(location, shadowCopy)
        {
        }
    }
}
