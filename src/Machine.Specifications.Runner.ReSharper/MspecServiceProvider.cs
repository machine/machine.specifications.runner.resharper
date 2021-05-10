using System;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Assemblies.Impl;
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

        private readonly IUnitTestingSettings settings;

        private readonly Lazy<MspecOutOfProcessUnitTestRunStrategy> legacyRunStrategy;

        private readonly Lazy<MspecTestRunnerRunStrategy> runStrategy;

        public MspecServiceProvider(
            MspecTestProvider provider,
            ISolution solution,
            UnitTestingCachingService cachingService,
            IUnitTestElementManager elementManager,
            IUnitTestElementIdFactory elementIdFactory,
            IUnitTestingSettings settings,
            ResolveContextManager resolveContextManager)
        {
            this.elementIdFactory = elementIdFactory;
            this.settings = settings;

            legacyRunStrategy = Lazy.Of(solution.GetComponent<MspecOutOfProcessUnitTestRunStrategy>, true);
            runStrategy = Lazy.Of(solution.GetComponent<MspecTestRunnerRunStrategy>, true);

            Provider = provider;
            CachingService = cachingService;
            ElementManager = elementManager;
            ResolveContextManager = resolveContextManager;
        }

        public MspecTestProvider Provider { get; }

        public UnitTestingCachingService CachingService { get; }

        public IUnitTestElementManager ElementManager { get; }

        public ResolveContextManager ResolveContextManager { get; }

        public IUnitTestRunStrategy GetRunStrategy(IUnitTestElement element)
        {
            var project = element.Id.Project;
            var targetFrameworkId = element.Id.TargetFrameworkId;

            var isNetFramework = targetFrameworkId.IsNetFramework || !project.IsDotNetCoreProject() || !targetFrameworkId.IsNetCoreApp;

            if (isNetFramework && settings.TestRunner.UseLegacyRunner.Value)
            {
                return legacyRunStrategy.Value;
            }

            return runStrategy.Value;
        }

        public UnitTestElementId CreateId(IProject project, TargetFrameworkId targetFrameworkId, string id)
        {
            return elementIdFactory.Create(Provider, project, targetFrameworkId, id);
        }
    }
}
