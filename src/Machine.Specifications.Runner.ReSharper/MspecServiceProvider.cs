using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Assemblies.Impl;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Machine.Specifications.Runner.ReSharper.RunStrategies;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecServiceProvider
    {
        private readonly MspecOutOfProcessUnitTestRunStrategy processUnitTestRunStrategy;

        private readonly MspecTestProvider provider;

        private readonly ISolution solution;

        private readonly IUnitTestElementIdFactory elementIdFactory;

        private readonly IUnitTestingSettings settings;

        public MspecServiceProvider(
            MspecTestProvider provider,
            ISolution solution,
            UnitTestingCachingService cachingService,
            IUnitTestElementManager elementManager,
            IUnitTestElementIdFactory elementIdFactory,
            IUnitTestingSettings settings,
            MspecOutOfProcessUnitTestRunStrategy processUnitTestRunStrategy,
            ResolveContextManager resolveContextManager)
        {
            this.provider = provider;
            this.solution = solution;
            this.elementIdFactory = elementIdFactory;
            this.settings = settings;
            this.processUnitTestRunStrategy = processUnitTestRunStrategy;

            CachingService = cachingService;
            ElementManager = elementManager;
            ResolveContextManager = resolveContextManager;
        }

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
                return processUnitTestRunStrategy;
            }

            return solution.GetComponent<MspecTestRunnerRunStrategy>();
        }

        public UnitTestElementId CreateId(IProject project, TargetFrameworkId targetFrameworkId, string id)
        {
            return elementIdFactory.Create(provider, project, targetFrameworkId, id);
        }
    }
}
