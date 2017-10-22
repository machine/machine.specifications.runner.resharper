using System.Linq;
using JetBrains.DataFlow;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using Machine.Specifications.ReSharperProvider.RunStrategies;

namespace Machine.Specifications.ReSharperProvider
{
    [SolutionComponent]
    public class MspecServiceProvider
    {
        private static readonly MspecOutOfProcessUnitTestRunStrategy Default = new MspecOutOfProcessUnitTestRunStrategy();

        private readonly MspecTestProvider _provider;
        private readonly ISolution _solution;
        private readonly IUnitTestElementIdFactory _elementIdFactory;
        private readonly IDotNetCoreSdkResolver _dotNetCoreSdkResolver;

        public MspecServiceProvider(
            MspecTestProvider provider,
            ISolution solution,
            UnitTestingCachingService cachingService,
            Lifetime lifetime,
            IUnitTestElementManager elementManager,
            IUnitTestElementIdFactory elementIdFactory,
            IUnitTestElementCategoryFactory categoryFactory,
            IDotNetCoreSdkResolver dotNetCoreSdkResolver)
        {
            _provider = provider;
            _solution = solution;
            _elementIdFactory = elementIdFactory;
            _dotNetCoreSdkResolver = dotNetCoreSdkResolver;
            CategoryFactory = categoryFactory;
            CachingService = cachingService;
            ElementManager = elementManager;

            AddElementHandler(lifetime);
        }

        public UnitTestingCachingService CachingService { get; }

        public IUnitTestElementManager ElementManager { get; }

        public IUnitTestElementCategoryFactory CategoryFactory { get; }

        public IUnitTestRunStrategy GetRunStrategy(IUnitTestElement element)
        {
            var project = element.Id.Project;

            if (!project.IsDotNetCoreProject() || element.Id.TargetFrameworkId.IsNetFramework)
                return Default;

            if (_dotNetCoreSdkResolver.GetVersion(project) < ImportantSdkVersions.VsTestVersion)
                return _solution.GetComponent<MspecDotNetTestRunStrategy>();

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
