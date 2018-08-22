using System.Linq;
using JetBrains.DataFlow;
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
            Lifetime lifetime,
            IUnitTestElementManager elementManager,
            IUnitTestElementIdFactory elementIdFactory,
            IUnitTestElementCategoryFactory categoryFactory,
            MspecOutOfProcessUnitTestRunStrategy processUnitTestRunStrategy)
        {
            _provider = provider;
            _solution = solution;
            _elementIdFactory = elementIdFactory;
            CategoryFactory = categoryFactory;
            CachingService = cachingService;
            ElementManager = elementManager;

            AddElementHandler(lifetime);
            _processUnitTestRunStrategy = processUnitTestRunStrategy;
        }

        public UnitTestingCachingService CachingService { get; }

        public IUnitTestElementManager ElementManager { get; }

        public IUnitTestElementCategoryFactory CategoryFactory { get; }

        public IUnitTestRunStrategy GetRunStrategy(IUnitTestElement element)
        {
            var project = element.Id.Project;

            if (!project.IsDotNetCoreProject() || element.Id.TargetFrameworkId.IsNetFramework)
                return _processUnitTestRunStrategy;

            return _solution.GetComponent<MspecDotNetVsTestRunStrategy>();
        }

        public UnitTestElementId CreateId(IProject project, TargetFrameworkId targetFrameworkId, string id)
        {
            return _elementIdFactory.Create(_provider, project, targetFrameworkId, id);
        }

        private void AddElementHandler(Lifetime lifetime)
        {
            ElementManager.UnitTestElementsRemoved.Advise(lifetime, set =>
            {
                foreach (var element in set)
                    ElementManager.RemoveElements(element.Children.ToSet());
            });
        }
    }
}
