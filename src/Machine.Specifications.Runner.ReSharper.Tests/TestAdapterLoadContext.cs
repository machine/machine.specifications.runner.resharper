using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class TestAdapterLoadContext : ITestAdapterLoadContext
    {
        private readonly IAssemblyResolver resolver;

        private readonly TestAdapterInfo adapter;

        private readonly List<Type> additionalTypes;

        public TestAdapterLoadContext(IAssemblyResolver resolver, TestAdapterInfo adapter)
        {
            this.resolver = resolver;
            this.adapter = adapter;

            //additionalTypes = adapter.AdditionalAssemblies
            //    .Select(resolver.LoadFrom)
            //    .SelectMany(x => x.GetExportedTypes())
            //    .ToList();
        }

        public ITestDiscoverer InitializeTestDiscoverer()
        {
            var type = resolver
                .LoadFrom(adapter.Discoverer.Assembly)
                .GetType(adapter.Discoverer.TypeName);

            return (ITestDiscoverer) Activator.CreateInstance(type);
        }

        public ITestExecutor InitializeTestExecutor()
        {
            var type = resolver
                .LoadFrom(adapter.Executor.Assembly)
                .GetType(adapter.Executor.TypeName);

            return (ITestExecutor) Activator.CreateInstance(type);
        }
    }
}
