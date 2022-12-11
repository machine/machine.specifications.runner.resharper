using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tasks;

[Serializable]
public class MspecTestContainer : TestContainer
{
    public MspecTestContainer(string location, ShadowCopy shadowCopy)
        : base(location, shadowCopy)
    {
    }
}
