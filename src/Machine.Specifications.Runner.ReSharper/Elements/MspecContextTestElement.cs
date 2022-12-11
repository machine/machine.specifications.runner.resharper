using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements;

public class MspecContextTestElement : ClrUnitTestElement.FromClass, IMspecTestElement
{
    [UsedImplicitly]
    public MspecContextTestElement()
    {
    }

    public MspecContextTestElement(IClrTypeName typeName, string? subject, string? ignoreReason)
        : base(typeName.FullName, typeName, GetDisplayName(typeName, subject))
    {
        Subject = subject;
        IgnoreReason = ignoreReason;
    }

    public override string Kind => "Context";

    [Persist]
    [UsedImplicitly]
    public string? Subject { get; set; }

    [Persist]
    [UsedImplicitly]
    public string? IgnoreReason { get; set; }

    private static string GetDisplayName(IClrTypeName typeName, string? subject)
    {
        var display = typeName.ShortName.ToFormat();

        return string.IsNullOrEmpty(subject)
            ? display
            : $"{subject}, {display}";
    }
}
