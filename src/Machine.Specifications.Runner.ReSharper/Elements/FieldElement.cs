using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public abstract class FieldElement : MspecTestElement
    {
        protected FieldElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string fieldName, string explicitReason)
            : base(services, id, typeName, explicitReason)
        {
            FieldName = fieldName;
            ShortName = fieldName.ToFormat();
        }

        public string FieldName { get; }

        protected virtual string GetTitlePrefix()
        {
            return string.Empty;
        }

        protected override string GetPresentation()
        {
            return $"{GetTitlePrefix()} {ShortName}".Trim();
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            var type = GetDeclaredType();

            if (type == null)
            {
                return null;
            }

            using (CompilationContextCookie.OverrideOrCreate(Id.Project.GetResolveContext(Id.TargetFrameworkId)))
            {
                return type
                    .EnumerateMembers(FieldName, type.CaseSensitiveName)
                    .OfType<IField>()
                    .FirstOrDefault();
            }
        }
    }
}
