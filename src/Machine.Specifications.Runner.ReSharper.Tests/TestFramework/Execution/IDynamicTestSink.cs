using System.Collections.Generic;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution
{
    public interface IDynamicTestSink
    {
        void Reset();

        void AddUnitTestElement(IUnitTestElement element);

        IEnumerable<IUnitTestElement> GetUnitTestElements();
    }
}
