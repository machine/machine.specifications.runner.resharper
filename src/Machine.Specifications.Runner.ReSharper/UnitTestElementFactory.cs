using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.Runner.ReSharper.Elements;

namespace Machine.Specifications.Runner.ReSharper
{
    public class UnitTestElementFactory
    {
        private readonly JetHashSet<IUnitTestElement> elements = new(UnitTestElement.Comparer.ByNaturalId);

        public MspecContextTestElement GetOrCreateContext(
            IClrTypeName typeName,
            string? subject,
            string[]? tags,
            string? ignoreReason)
        {
            var context = new MspecContextTestElement(typeName, subject, ignoreReason);

            if (tags != null)
            {
                context.OwnCategories = tags.Select(x => new UnitTestElementCategory(x)).ToJetHashSet();
            }

            return (MspecContextTestElement) elements.Intern(context);
        }

        public MspecSpecificationTestElement GetOrCreateSpecification(
            MspecContextTestElement context,
            string fieldName,
            string? behaviorType,
            string? ignoreReason)
        {
            var specification = new MspecSpecificationTestElement(context, fieldName, behaviorType, null, ignoreReason ?? context.IgnoreReason);

            return (MspecSpecificationTestElement) elements.Intern(specification);
        }

        public MspecBehaviorSpecificationTestElement GetOrCreateBehaviorSpecification(
            MspecSpecificationTestElement parent,
            string fieldName,
            string? ignoreReason)
        {
            var specification = new MspecBehaviorSpecificationTestElement(parent, fieldName, ignoreReason ?? parent.IgnoreReason);

            return (MspecBehaviorSpecificationTestElement) elements.Intern(specification);
        }
    }
}
