using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SolutionComponent]
    public class TestAdapterLoadContextFactory : ITestAdapterLoadContextFactory
    {
        private readonly IAssemblyResolver resolver;

        private TestAdapterInfo adapter;

        public TestAdapterLoadContextFactory(IAssemblyResolver resolver)
        {
            this.resolver = resolver;
        }

        public void SetTestAdapterLoader(TestAdapterLoader loader)
        {
            if (loader is TestAdapterInfo info)
            {
                adapter = info;
            }
        }

        public ITestAdapterLoadContext Initialize()
        {
            return new TestAdapterLoadContext(resolver, adapter);
        }
    }
}
