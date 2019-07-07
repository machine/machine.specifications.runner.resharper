using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.Common;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestElementMapperFactory : ITestElementMapperFactory
    {
        private readonly MspecServiceProvider _serviceProvider;

        public MspecTestElementMapperFactory(MspecServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITestElementMapper Create(UnitTestElementOrigin origin, IProject project, TargetFrameworkId targetFrameworkId)
        {
            var factory = new UnitTestElementFactory(_serviceProvider, targetFrameworkId);

            return new MspecTestElementMapper(project, origin, targetFrameworkId, factory);
        }
    }
}
