using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Channel.Json;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetTest;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.RunStrategies
{
    [SolutionComponent]
    public class MspecDotNetTestRunStrategy : DotNetTestRunStrategy
    {
        public MspecDotNetTestRunStrategy(
            [NotNull] IJsonBasedUnitTestServerFactory serverFactory,
            [NotNull] MspecTestElementMapperFactory mapperFactory,
            [NotNull] IDotNetTestCaseMap testCaseMap,
            [NotNull] IUnitTestResultManager resultManager,
            [NotNull] ISolutionProcessStartInfoPatcher processStartInfoPatcher,
            [NotNull] ILogger logger)
            : base(serverFactory, mapperFactory, testCaseMap, resultManager, processStartInfoPatcher, logger)
        {
        }
    }
}
