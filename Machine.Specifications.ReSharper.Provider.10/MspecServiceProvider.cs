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
        private readonly IUnitTestElementIdFactory _elementIdFactory;

        public MspecServiceProvider(
            MspecTestProvider provider,
            UnitTestingCachingService cachingService,
            IUnitTestElementManager elementManager,
            IUnitTestElementIdFactory elementIdFactory,
            IUnitTestElementCategoryFactory categoryFactory)
        {
            _provider = provider;
            _elementIdFactory = elementIdFactory;
            CategoryFactory = categoryFactory;
            CachingService = cachingService;
            ElementManager = elementManager;
        }

        public UnitTestingCachingService CachingService { get; }

        public IUnitTestElementManager ElementManager { get; }

        public IUnitTestElementCategoryFactory CategoryFactory { get; }

        public IUnitTestRunStrategy GetRunStrategy(IUnitTestElement element)
        {
            return Default;
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
