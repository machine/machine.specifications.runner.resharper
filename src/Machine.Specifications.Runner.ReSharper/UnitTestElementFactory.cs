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
            UpdateCategories(context, tags);

            return (MspecContextTestElement) elements.Intern(context);
        }

        public MspecBehaviorTestElement GetOrCreateBehavior(
            MspecContextTestElement context,
            string fieldName,
            string? ignoreReason)
        {
            var behavior = new MspecBehaviorTestElement(context, fieldName, null, ignoreReason);

            return (MspecBehaviorTestElement) elements.Intern(behavior);
        }

        public MspecContextSpecificationTestElement GetOrCreateContextSpecification(
            MspecContextTestElement context,
            string fieldName,
            string? ignoreReason)
        {
            var specification = new MspecContextSpecificationTestElement(context, fieldName, null, ignoreReason);

            return (MspecContextSpecificationTestElement) elements.Intern(specification);
        }

        public MspecBehaviorSpecificationTestElement GetOrCreateBehaviorSpecification(
            MspecBehaviorTestElement behavior,
            string fieldName,
            string? ignoreReason)
        {
            var specification = new MspecBehaviorSpecificationTestElement(behavior.Context, behavior, fieldName, ignoreReason);

            return (MspecBehaviorSpecificationTestElement) elements.Intern(specification);
        }

        private void UpdateCategories(UnitTestElement element, string[]? categories)
        {
            if (categories != null)
            {
                element.OwnCategories = categories.Select(x => new UnitTestElementCategory(x)).ToJetHashSet();
            }
        }
    }
}
