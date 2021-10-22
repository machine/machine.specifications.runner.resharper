using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.TestFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.Util;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [TestFixture]
    [Category("Unit Test support")]
    [EnsureUnitTestRepositoryIsEmpty]
    public abstract class UnitTestElementDiscoveryTestBase : BaseTestWithSingleProject
    {
        protected void DumpElements(IEnumerable<IUnitTestElement> elements, string path)
        {
            using (UT.ReadLock())
            {
                var reportedElements = new HashSet<IUnitTestElement>(elements);
                var roots = new HashSet<IUnitTestElement>();
                var orphanedElements = new HashSet<IUnitTestElement>();
                var invalidChildren = new HashSet<IUnitTestElement>();

                foreach (var unitTestElement in reportedElements)
                {
                    if (unitTestElement.Parent != null && !reportedElements.Contains(unitTestElement.Parent))
                    {
                        orphanedElements.Add(unitTestElement);
                    }

                    if (unitTestElement.Parent != null && !unitTestElement.Parent.Children.Contains(unitTestElement))
                    {
                        invalidChildren.Add(unitTestElement);
                    }

                    if (unitTestElement.Parent == null)
                    {
                        roots.Add(unitTestElement);
                    }
                }

                ExecuteWithGold(path, file =>
                {
                    foreach (var element in OrderById(roots))
                    {
                        DumpElement(file, element, reportedElements, string.Empty);
                    }

                    if (orphanedElements.Count > 0)
                    {
                        file.WriteLine();
                        file.WriteLine();
                        file.WriteLine("Section: Has parent, but parent wasn't reported");
                        file.WriteLine();

                        foreach (var element in OrderById(orphanedElements))
                        {
                            DumpElement(file, element, reportedElements, string.Empty);
                        }
                    }

                    if (orphanedElements.Count <= 0)
                    {
                        return;
                    }

                    file.WriteLine();
                    file.WriteLine();
                    file.WriteLine("Section: Has parent, but parent doesn't have it among children");
                    file.WriteLine();

                    foreach (var element in OrderById(invalidChildren))
                    {
                        DumpElement(file, element, reportedElements, string.Empty);
                    }
                });
            }
        }

        protected virtual void DumpElement(TextWriter file, IUnitTestElement element, ISet<IUnitTestElement> reported, string indent)
        {
            var isReported = reported.Contains(element)
                ? string.Empty
                : "NOT REPORTED";

            file.WriteLine(indent + GetIdString(element) + ":" + isReported);
            file.WriteLine(indent + "  Kind: " + element.Kind);
            file.WriteLine(indent + "  DisplayName: " + element.GetPresentation());
            file.WriteLine(indent + "  Categories: {0}", element.Categories().ToSet().AggregateString(",", (builder, category) => builder.Append(category.Name)));
            file.WriteLine();

            foreach (var element1 in OrderById(element.Children))
            {
                DumpElement(file, element1, reported, indent + "  ");
            }
        }

        protected IEnumerable<IUnitTestElement> OrderById(IEnumerable<IUnitTestElement> elements)
        {
            return elements.OrderBy(GetIdString);
        }

        protected virtual string GetIdString(IUnitTestElement element) => element.Id.ToString();
    }
}
