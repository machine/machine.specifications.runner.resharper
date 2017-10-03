using JetBrains.Application.ProcessRunner;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.Channel;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetTest;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider
{
    [SolutionComponent]
    public class MspecTestArtefactExplorer : DotNetTestArtefactExplorer<MspecTestProvider>
    {
        public MspecTestArtefactExplorer(
            MspecTestProvider provider, 
            IDotNetCoreSdkResolver sdkResolver, 
            IDotNetCoreUnitTestServerFactory serverFactory, 
            MspecTestElementMapperFactory mapperFactory, 
            IProcessRunnerManager processRunnerManager, 
            IDotNetTestCaseMap testCaseMap, 
            ILogger logger) 
            : base(provider, sdkResolver, serverFactory, mapperFactory, processRunnerManager, testCaseMap, logger)
        {
        }
    }
}