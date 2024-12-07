using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tasks;

[Serializable]
public abstract class MspecRemoteTask(
    string testId,
    string? ignoreReason,
    bool runAllChildren = true,
    bool runExplicitly = false)
    : RemoteTask
{
    public string TestId { get; } = testId;

    public string? IgnoreReason { get; } = ignoreReason;

    public bool RunAllChildren { get; } = runAllChildren;

    public bool RunExplicitly { get; } = runExplicitly;
}
