using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
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

        public MspecBehaviorSpecificationTestElement(MspecBehaviorTestElement behavior, string fieldName, string? ignoreReason)
            : base($"{behavior.Context.TypeName.FullName}.{fieldName}", behavior)
        {
            FieldName = fieldName;
            DisplayName = fieldName.ToFormat();
            IgnoreReason = ignoreReason;
        }

        public MspecBehaviorTestElement? Behavior => Parent as MspecBehaviorTestElement;

        public override string Kind => "Behavior Specification";

        public bool IsNotRunnableStandalone => Origin == UnitTestElementOrigin.Dynamic;

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

        public override IEnumerable<UnitTestElementLocation> GetLocations()
        {
            return Behavior!.GetLocations();
        }

        public override IDeclaredElement? GetDeclaredElement()
        {
            var contextElement = Behavior!.Context.GetDeclaredElement();

            if (contextElement is not IClass type)
            {
                return null;
            }

            using (CompilationContextCookie.OverrideOrCreate(Project.GetResolveContext(TargetFrameworkId)))
            {
                return type
                    .EnumerateMembers<IField>(FieldName, type.CaseSensitiveName)
                    .FirstOrDefault();
            }
        }
    }
}
