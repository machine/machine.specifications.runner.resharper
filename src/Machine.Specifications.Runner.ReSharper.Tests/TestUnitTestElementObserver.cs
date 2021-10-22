using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class TestUnitTestElementObserver : IUnitTestElementObserverOnFile
    {
        private readonly OrderedHashSet<IUnitTestElement> elements = new OrderedHashSet<IUnitTestElement>();

        public TestUnitTestElementObserver(IUnitTestElementSource source)
        {
            Source = source;
        }

        public IUnitTestElementSource Source { get; }

        public ICollection<IUnitTestElement> Elements => elements;

        public T GetElementById<T>(string testId)
        {
            return (T) elements.FirstOrDefault(x => x.NaturalId.TestId == testId);
        }

        public void OnUnitTestElement(IUnitTestElement element)
        {
            elements.Add(element);
        }

        public void OnUnitTestElementDisposition(IUnitTestLikeElement element, TextRange navigationRange, TextRange containingRange)
        {
            foreach (var relatedElement in element.GetRelatedUnitTestElements())
            {
                elements.Add(relatedElement);
            }
        }
    }
}
