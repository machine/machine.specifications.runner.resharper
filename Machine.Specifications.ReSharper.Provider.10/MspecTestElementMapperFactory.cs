using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetTest;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetVsTest;

namespace Machine.Specifications.ReSharperProvider
{
    [SolutionComponent]
    public class MspecTestElementMapperFactory : IDotNetVsTestElementMapperFactory, IDotNetTestElementMapperFactory
    {
        private readonly MspecServiceProvider _serviceProvider;

        public MspecTestElementMapperFactory(MspecServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private MspecTestElementMapper Create(IProject project, TargetFrameworkId targetFrameworkId)
        {
            var factory = new UnitTestElementFactory(_serviceProvider, targetFrameworkId);

            return new MspecTestElementMapper(project, targetFrameworkId, factory);
        }

        IDotNetVsTestElementMapper IDotNetVsTestElementMapperFactory.Create(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return Create(project, targetFrameworkId);
        }

        IDotNetTestElementMapper IDotNetTestElementMapperFactory.Create(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return Create(project, targetFrameworkId);
        }
    }
}
