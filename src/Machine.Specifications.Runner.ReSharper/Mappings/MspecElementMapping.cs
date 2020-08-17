using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    public abstract class MspecElementMapping<TElement, TTask> : IUnitTestElementToRemoteTaskMapping<TElement>, IRemoteTaskToUnitTestElementMapping<TTask>
        where TElement : IUnitTestElement
        where TTask : RemoteTask
    {
        protected MspecElementMapping(MspecServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected MspecServiceProvider ServiceProvider { get; }

        public abstract TTask ToRemoteTask(TElement element, IUnitTestRun run);

        public abstract TElement ToElement(TTask task, IUnitTestRun run);

        protected UnitTestElementFactory GetFactory(IUnitTestRun run)
        {
            return run.GetOrCreateDataUnderLock(
                MspecElementMappingKeys.ElementFactoryKey,
                () => new UnitTestElementFactory(ServiceProvider, run.TargetFrameworkId, null, UnitTestElementOrigin.Dynamic));
        }

        public RemoteTask GetRemoteTask(TElement element, IUnitTestRun run)
        {
            return ToRemoteTask(element, run);
        }

        public IUnitTestElement GetElement(TTask task, IUnitTestRun run)
        {
            return ToElement(task, run);
        }
    }
}
