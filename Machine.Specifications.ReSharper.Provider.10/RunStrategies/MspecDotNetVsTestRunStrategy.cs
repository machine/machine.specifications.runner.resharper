using JetBrains.Application.ProcessRunner;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.Channel;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetVsTest;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.RunStrategies
{
    [SolutionComponent]
    public class MspecDotNetVsTestRunStrategy : DotNetVsTestRunStrategy
    {
        public MspecDotNetVsTestRunStrategy(
            IDotNetCoreUnitTestServerFactory serverFactory,
            MspecTestElementMapperFactory mapperFactory, 
            IDotNetVsTestCaseMapProvider testCaseMapProvider, 
            IUnitTestResultManager resultManager, 
            IProcessRunnerManager processRunner, 
            ILogger logger) 
            : base(serverFactory, mapperFactory, testCaseMapProvider, resultManager, processRunner, logger)
        {
        }
    }
}
