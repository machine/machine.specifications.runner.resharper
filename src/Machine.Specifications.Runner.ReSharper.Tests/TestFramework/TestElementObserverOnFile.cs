using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework
{
    public class TestElementObserverOnFile : IUnitTestElementObserverOnFile 
    {
        private readonly IUnitTestElementObserver inner;

        public TestElementObserverOnFile(IUnitTestElementObserver inner)
        {
            this.inner = inner;
        }

        public IUnitTestElementSource Source => inner.Source;

        public T GetElementById<T>(string testId)
        {
            return inner.GetElementById<T>(testId);
        }

        public void OnUnitTestElement(IUnitTestElement element)
        {
            inner.OnUnitTestElement(element);
        }

        public void OnUnitTestElementDisposition(IUnitTestLikeElement element, TextRange navigationRange, TextRange containingRange)
        {
        }
    }
}
