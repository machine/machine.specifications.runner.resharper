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
        private readonly MspecOutOfProcessUnitTestRunStrategy _processUnitTestRunStrategy;

        private readonly MspecTestProvider _provider;
        private readonly ISolution _solution;
        private readonly IUnitTestElementIdFactory _elementIdFactory;
        private readonly IUnitTestingSettings _settings;

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
            _provider = provider;
            _solution = solution;
            _elementIdFactory = elementIdFactory;
            _settings = settings;
            _processUnitTestRunStrategy = processUnitTestRunStrategy;

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

            if (isNetFramework && _settings.TestRunner.UseLegacyRunner.Value)
            {
                return _processUnitTestRunStrategy;
            }

            return _solution.GetComponent<MspecTestRunnerRunStrategy>();
        }

        public UnitTestElementId CreateId(IProject project, TargetFrameworkId targetFrameworkId, string id)
        {
            return _elementIdFactory.Create(_provider, project, targetFrameworkId, id);
        }
    }
}
