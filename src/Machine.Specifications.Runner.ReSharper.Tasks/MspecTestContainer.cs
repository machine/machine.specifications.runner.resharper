using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tasks;

[Serializable]
public class MspecTestContainer(string location, ShadowCopy shadowCopy) : TestContainer(location, shadowCopy);
