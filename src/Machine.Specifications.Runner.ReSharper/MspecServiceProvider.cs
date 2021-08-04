using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Machine.Specifications.Runner.ReSharper.RunStrategies;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecServiceProvider
    {
        private readonly IUnitTestElementIdFactory elementIdFactory;

        private readonly Lazy<MspecTestRunnerRunStrategy> runStrategy;

        public MspecServiceProvider(
            MspecTestProvider provider,
            ISolution solution,
            UnitTestingCachingService cachingService,
            IUnitTestElementManager elementManager,
            IUnitTestElementIdFactory elementIdFactory)
        {
            this.elementIdFactory = elementIdFactory;

            runStrategy = Lazy.Of(solution.GetComponent<MspecTestRunnerRunStrategy>, true);

            Provider = provider;
            CachingService = cachingService;
            ElementManager = elementManager;
        }

        public MspecTestProvider Provider { get; }

        public UnitTestingCachingService CachingService { get; }

        public IUnitTestElementManager ElementManager { get; }

        public IUnitTestRunStrategy GetRunStrategy()
        {
            return runStrategy.Value;
        }

        public UnitTestElementId CreateId(IProject project, TargetFrameworkId targetFrameworkId, string id)
        {
            return elementIdFactory.Create(Provider, project, targetFrameworkId, id);
        }
    }
}
