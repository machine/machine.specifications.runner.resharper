using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.Common;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.ReSharperProvider
{
    [SolutionComponent]
    public class MspecTestElementMapperFactory : ITestElementMapperFactory
    {
        private readonly MspecServiceProvider _serviceProvider;

        public MspecTestElementMapperFactory(MspecServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private MspecTestElementMapper CreateMapper(IProject project, TargetFrameworkId targetFrameworkId)
        {
            var factory = new UnitTestElementFactory(_serviceProvider, targetFrameworkId);

            return new MspecTestElementMapper(project, targetFrameworkId, factory);
        }

        public ITestElementMapper Create(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return CreateMapper(project, targetFrameworkId);
        }
    }
}
