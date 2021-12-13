using System.Collections.Generic;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Execution.Hosting;
using JetBrains.ReSharper.UnitTestFramework.Execution.Launch.Rules;
using JetBrains.ReSharper.UnitTestFramework.Session;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;

namespace Machine.Specifications.Runner.ReSharper.Rules
{
    [UnitTestElementsTransformationRule(Priority = 90)]
    public class EnsureAncestorsAddedToExecutedElementsRule : IUnitTestElementsTransformationRule
    {
        public IUnitTestElementCriterion Apply(
            IUnitTestElementCriterion criterion,
            IUnitTestSession session,
            IHostProvider hostProvider)
        {
            return criterion;
        }

        public void Apply(ISet<IUnitTestElement> elements, IUnitTestSession session, IHostProvider hostProvider)
        {
            var additional = new HashSet<IUnitTestElement>();

            foreach (var element in elements)
            {
                if (element is IMspecTestElement)
                {
                    var parent = element.Parent;

                    while (parent != null && additional.Add(parent))
                    {
                        parent = parent.Parent;
                    }
                }
            }

            elements.AddRange(additional);
        }
    }
}
