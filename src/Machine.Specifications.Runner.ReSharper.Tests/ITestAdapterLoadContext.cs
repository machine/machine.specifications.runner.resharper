using JetBrains.ReSharper.TestRunner.Abstractions;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public interface ITestAdapterLoadContext
    {
        ITestDiscoverer InitializeTestDiscoverer();

        ITestExecutor InitializeTestExecutor();
    }
}
