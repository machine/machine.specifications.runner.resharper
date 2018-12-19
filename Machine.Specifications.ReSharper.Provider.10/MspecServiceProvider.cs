using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Machine.Specifications.ReSharperProvider.RunStrategies;

namespace Machine.Specifications.ReSharperProvider
{
    [SolutionComponent]
    public class MspecServiceProvider
    {
        private readonly MspecOutOfProcessUnitTestRunStrategy _processUnitTestRunStrategy;

        private readonly MspecTestProvider _provider;
        private readonly ISolution _solution;
        private readonly IUnitTestElementIdFactory _elementIdFactory;

        public MspecServiceProvider(
            MspecTestProvider provider,
            ISolution solution,
            UnitTestingCachingService cachingService,
            IUnitTestElementManager elementManager,
            IUnitTestElementIdFactory elementIdFactory,
            MspecOutOfProcessUnitTestRunStrategy processUnitTestRunStrategy)
        {
            _provider = provider;
            _solution = solution;
            _elementIdFactory = elementIdFactory;
            _processUnitTestRunStrategy = processUnitTestRunStrategy;

            CachingService = cachingService;
            ElementManager = elementManager;
        }

        public UnitTestingCachingService CachingService { get; }

        public IUnitTestElementManager ElementManager { get; }
        
        public IUnitTestRunStrategy GetRunStrategy(IUnitTestElement element)
        {
            var project = element.Id.Project;
            var targetFrameworkId = element.Id.TargetFrameworkId;

            if (targetFrameworkId.IsNetFramework || !project.IsDotNetCoreProject() || !targetFrameworkId.IsNetCoreApp)
                return _processUnitTestRunStrategy;

            return _solution.GetComponent<MspecDotNetVsTestRunStrategy>();
        }

        public UnitTestElementId CreateId(IProject project, TargetFrameworkId targetFrameworkId, string id)
        {
            return _elementIdFactory.Create(_provider, project, targetFrameworkId, id);
        }

        public IUnitTestElement GetElementById(IProject project, TargetFrameworkId targetFrameworkId, string id)
        {
            return ElementManager.GetElementById(CreateId(project, targetFrameworkId, id));
        }
    }
}
