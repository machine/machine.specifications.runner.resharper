using JetBrains.Application.ProcessRunner;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.Channel;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetTest;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.RunStrategies
{
    [SolutionComponent]
    public class MspecDotNetTestRunStrategy : DotNetTestRunStrategy
    {
        public MspecDotNetTestRunStrategy(
            IDotNetCoreUnitTestServerFactory serverFactory,
            MspecTestElementMapperFactory mapperFactory, 
            IDotNetTestCaseMap testCaseMap, 
            IUnitTestResultManager resultManager, 
            IProcessRunnerManager processRunner, 
            ILogger logger) 
            : base(serverFactory, mapperFactory, testCaseMap, resultManager, processRunner, logger)
        {
        }
    }
}