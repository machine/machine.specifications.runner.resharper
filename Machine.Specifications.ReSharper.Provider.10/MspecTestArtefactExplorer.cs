using JetBrains.Application.Processes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Channel.Json;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore;
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
            IJsonBasedUnitTestServerFactory serverFactory, 
            MspecTestElementMapperFactory mapperFactory, 
            ISolutionProcessStartInfoPatcher processStartInfoPatcher,
            IDotNetTestCaseMap testCaseMap, 
            ILogger logger) 
            : base(provider, sdkResolver, serverFactory, mapperFactory, processStartInfoPatcher, testCaseMap, logger)
        {
        }
    }
}
