using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public abstract class FieldElement : Element
    {
        protected FieldElement(
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored)
            : base(id, parent, typeName, serviceProvider, isIgnored || parent.Explicit)
        {
            FieldName = fieldName;
        }

        public override string ShortName => FieldName;

        public string FieldName { get; }

        protected virtual string GetTitlePrefix()
        {
            return string.Empty;
        }

        protected override string GetPresentation()
        {
            return $"{GetTitlePrefix()} {FieldName.ToFormat()}".Trim();
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            var type = GetDeclaredType();

            if (type == null)
            {
                return null;
            }

            using (CompilationContextCookie.OverrideOrCreate(ServiceProvider.ResolveContextManager.GetOrCreateProjectResolveContext(Id.Project, Id.TargetFrameworkId)))
            {
                return type
                    .EnumerateMembers(FieldName, type.CaseSensitiveName)
                    .OfType<IField>()
                    .FirstOrDefault();
            }
        }
    }
}
