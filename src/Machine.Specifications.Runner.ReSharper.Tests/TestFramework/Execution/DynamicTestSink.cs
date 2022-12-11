using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution;

[SolutionComponent]
public class DynamicTestSink : IDynamicTestSink
{
    private readonly Dictionary<string, IUnitTestElement> elements = new();

    public void Reset()
    {
        elements.Clear();
    }

    public void AddUnitTestElement(IUnitTestElement element)
    {
        elements[element.NaturalId.TestId] = element;
    }

    public IEnumerable<IUnitTestElement> GetUnitTestElements()
    {
        return elements.Values;
    }
}
