using System.Collections.Generic;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Rules;
using Machine.Specifications.Runner.ReSharper.Tests.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Rules
{
    [TestFixture]
    public class EnsureAncestorsAddedToExecutedElementsRuleTests : UnitTestElementTestBase
    {
        [Test]
        public void UnreportedParentsAreAddedToSet()
        {
            WithDiscovery(() =>
            {
                var context = new MspecContextTestElement(new ClrTypeName("Namespace.Context"), null, null);
                var behavior = new MspecSpecificationTestElement(context, "behaves_like", "Namespace.ABehavior", null, null);
                var specification = new MspecBehaviorSpecificationTestElement(behavior, "should", null);

                var elements = new HashSet<IUnitTestElement>
                {
                    context,
                    specification
                };

                var rule = new EnsureAncestorsAddedToExecutedElementsRule();

                rule.Apply(elements, null!, null!);

                CollectionAssert.Contains(elements, behavior);
            });
        }
    }
}
