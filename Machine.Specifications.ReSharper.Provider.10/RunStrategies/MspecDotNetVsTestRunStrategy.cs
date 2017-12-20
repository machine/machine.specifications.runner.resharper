using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Channel.Json;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetVsTest;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.RunStrategies
{
    [SolutionComponent]
    public class MspecDotNetVsTestRunStrategy : DotNetVsTestRunStrategy
    {
        public MspecDotNetVsTestRunStrategy(
            [NotNull] IJsonBasedUnitTestServerFactory serverFactory, 
            [NotNull] IDotNetVsTestElementMapperFactory mapperFactory, 
            [NotNull] IDotNetVsTestRunSettingsProvider runSettingsProvider, 
            [NotNull] IDotNetVsTestCaseMapProvider testCaseMapProvider, 
            [NotNull] IUnitTestResultManager resultManager, 
            [NotNull] ISolutionProcessStartInfoPatcher processStartInfoPatcher, 
            [NotNull] ILogger logger, 
            [NotNull] IUnitTestingSettings unitTestingSettings, 
            [NotNull] IEnumerable<IRunSettingsPostProcessor> runSettingsPostProcessors) 
            : base(serverFactory, mapperFactory, runSettingsProvider, testCaseMapProvider, resultManager, processStartInfoPatcher, logger, unitTestingSettings, runSettingsPostProcessors)
        {
        }
    }
}
