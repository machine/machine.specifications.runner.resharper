using JetBrains.Annotations;
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
            [NotNull] MspecTestProvider provider, 
            [NotNull] IDotNetCoreSdkResolver sdkResolver, 
            [NotNull] IJsonBasedUnitTestServerFactory serverFactory, 
            [NotNull] MspecTestElementMapperFactory mapperFactory, 
            [NotNull] ISolutionProcessStartInfoPatcher processStartInfoPatcher, 
            [NotNull] IDotNetCoreTestCaseMap testCaseMap, 
            [NotNull] ILogger logger) 
            : base(provider, sdkResolver, serverFactory, mapperFactory, processStartInfoPatcher, testCaseMap, logger)
        {
        }
    }
}
