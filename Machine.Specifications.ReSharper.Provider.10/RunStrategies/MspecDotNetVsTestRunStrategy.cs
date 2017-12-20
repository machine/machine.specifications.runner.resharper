using System.Collections.Generic;
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
            IJsonBasedUnitTestServerFactory serverFactory, 
            MspecTestElementMapperFactory mapperFactory, 
            DefaultDotNetVsTestRunSettingsProvider runSettingsProvider,
            IDotNetVsTestCaseMapProvider testCaseMapProvider, 
            IUnitTestResultManager resultManager, 
            ISolutionProcessStartInfoPatcher processStartInfoPatcher, 
            ILogger logger, 
            IUnitTestingSettings unitTestingSettings, 
            IEnumerable<IRunSettingsPostProcessor> runSettingsPostProcessors) 
            : base(serverFactory, mapperFactory, runSettingsProvider, testCaseMapProvider, resultManager, processStartInfoPatcher, logger, unitTestingSettings, runSettingsPostProcessors)
        {
        }
    }
}
