using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    public abstract class MspecElementMapping<TElement, TTask> : IUnitTestElementToRemoteTaskMapping<TElement>, IRemoteTaskToUnitTestElementMapping<TTask>
        where TElement : IUnitTestElement
        where TTask : RemoteTask
    {
        protected MspecElementMapping(MspecServiceProvider services)
        {
            Services = services;
        }

        protected MspecServiceProvider Services { get; }

        protected abstract TTask ToRemoteTask(TElement element, ITestRunnerExecutionContext context);

        protected abstract TElement? ToElement(TTask task, ITestRunnerDiscoveryContext context);

        public RemoteTask GetRemoteTask(TElement element, ITestRunnerExecutionContext context)
        {
            return ToRemoteTask(element, context);
        }

        public IUnitTestElement? GetElement(TTask task, ITestRunnerDiscoveryContext context)
        {
            return ToElement(task, context);
        }

        protected UnitTestElementFactory GetFactory(ITestRunnerDiscoveryContext context)
        {
            return context.GetOrCreateDataUnderLock(
                MspecElementMappingKeys.ElementFactoryKey,
                () => new UnitTestElementFactory(Services, context.TargetFrameworkId, null, context.Origin));
        }
    }
}
