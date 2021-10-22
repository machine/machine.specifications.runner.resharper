using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecContextTestElement : ClrUnitTestElement.FromClass
    {
        [UsedImplicitly]
        public MspecContextTestElement()
        {
        }

        public MspecContextTestElement(IClrTypeName typeName, string? subject, string? explicitReason)
            : base(typeName.FullName, typeName, GetDisplayName(typeName, subject))
        {
            Subject = subject;
            ExplicitReason = explicitReason;
        }

        public override string Kind => "Context";

        [Persist]
        [UsedImplicitly]
        public string? Subject { get; set; }

        [Persist]
        [UsedImplicitly]
        public string? ExplicitReason { get; set; }

        private static string GetDisplayName(IClrTypeName typeName, string? subject)
        {
            var display = typeName.ShortName.ToFormat();

            return string.IsNullOrEmpty(subject)
                ? display
                : $"{subject}, {display}";
        }
    }
}
