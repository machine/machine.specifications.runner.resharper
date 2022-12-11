using System;

namespace Machine.Specifications.Runner.ReSharper.Tasks;

[Serializable]
public class MspecSpecificationRemoteTask : MspecRemoteTask
{
    private MspecSpecificationRemoteTask(string testId, string? ignoreReason, bool runAllChildren, bool runExplicitly)
        : base(testId, ignoreReason, runAllChildren, runExplicitly)
    {
    }

    private MspecSpecificationRemoteTask(
        string contextTypeName,
        string fieldName,
        string? behaviorType,
        string? subject,
        string[]? tags,
        string? ignoreReason)
        : base($"{contextTypeName}.{fieldName}", ignoreReason)
    {
        ContextTypeName = contextTypeName;
        FieldName = fieldName;
        BehaviorType = behaviorType;
        Subject = subject;
        Tags = tags;
    }

    public string? ContextTypeName { get; set; }

    public string? BehaviorType { get; set; }

    public string? FieldName { get; set; }

    public string? Subject { get; }

    public string[]? Tags { get; set; }

    public static MspecSpecificationRemoteTask ToClient(
        string testId,
        string? ignoreReason,
        bool runAllChildren,
        bool runExplicitly)
    {
        return new(testId, ignoreReason, runAllChildren, runExplicitly);
    }

    public static MspecSpecificationRemoteTask ToServer(
        string contextTypeName,
        string fieldName,
        string? behaviorType,
        string? subject,
        string[]? tags,
        string? ignoreReason)
    {
        return new(contextTypeName, fieldName, behaviorType, subject, tags, ignoreReason);
    }
}
