using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public interface ITestAdapterLoadContextFactory
    {
        void SetTestAdapterLoader(TestAdapterLoader loader);

        ITestAdapterLoadContext Initialize();
    }
}
