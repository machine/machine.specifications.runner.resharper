using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecBehaviorSpecificationTestElement : ClrUnitTestElement.Row, ITestCase, IMspecTestElement
    {
        [UsedImplicitly]
        public MspecBehaviorSpecificationTestElement()
        {
        }

        public MspecBehaviorSpecificationTestElement(MspecSpecificationTestElement parent, string fieldName, string? ignoreReason)
            : base($"{parent.Context.TypeName.FullName}.{parent.FieldName}.{fieldName}", parent)
        {
            FieldName = fieldName;
            DisplayName = fieldName.ToFormat();
            IgnoreReason = ignoreReason;
        }

        public override string Kind => "Behavior Specification";

        public bool IsNotRunnableStandalone => Origin == UnitTestElementOrigin.Dynamic;

        public MspecSpecificationTestElement Specification => (MspecSpecificationTestElement) Parent!;

        [Persist]
        [UsedImplicitly]
        public string FieldName { get; set; } = null!;

        [Persist]
        [UsedImplicitly]
        public string DisplayName { get; set; } = null!;

        public override string ShortName => DisplayName;

        [Persist]
        [UsedImplicitly]
        public string? IgnoreReason { get; set; }

        public override IDeclaredElement? GetDeclaredElement()
        {
            if (Specification.BehaviorType == null)
            {
                return null;
            }

            using (CompilationContextCookie.OverrideOrCreate(Project.GetResolveContext(TargetFrameworkId)))
            {
                var behaviorType = UT.Facade.TypeCache.GetTypeElement(
                    Project,
                    TargetFrameworkId,
                    new ClrTypeName(Specification.BehaviorType),
                    false,
                    true);

                return behaviorType?
                    .EnumerateMembers<IField>(FieldName, behaviorType.CaseSensitiveName)
                    .FirstOrDefault();
            }
        }
    }
}
