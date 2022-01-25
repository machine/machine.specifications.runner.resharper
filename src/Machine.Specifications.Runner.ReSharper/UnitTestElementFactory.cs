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

        public MspecContextSpecificationTestElement GetOrCreateContextSpecification(
            MspecContextTestElement context,
            string fieldName,
            string? behaviorType,
            string? ignoreReason)
        {
            var specification = new MspecContextSpecificationTestElement(context, fieldName, behaviorType, null, ignoreReason);

            return (MspecContextSpecificationTestElement) elements.Intern(specification);
        }

        public MspecBehaviorSpecificationTestElement GetOrCreateBehaviorSpecification(
            MspecContextSpecificationTestElement parent,
            string fieldName,
            string? ignoreReason)
        {
            var specification = new MspecBehaviorSpecificationTestElement(parent, fieldName, ignoreReason);

            return (MspecBehaviorSpecificationTestElement) elements.Intern(specification);
        }
    }
}
