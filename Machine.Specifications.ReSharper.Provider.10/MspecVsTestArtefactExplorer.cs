using System.Collections.Generic;
using JetBrains.Application.Processes;
using JetBrains.DataFlow;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Channel.Json;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetVsTest;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider
{
    [SolutionComponent]
    public class MspecVsTestArtefactExplorer : DotNetVsTestArtefactExplorer<MspecTestProvider>
    {
        public MspecVsTestArtefactExplorer(
            Lifetime lifetime,
            MspecTestProvider provider,
            IDotNetCoreSdkResolver sdkResolver,
            IJsonBasedUnitTestServerFactory serverFactory,
            ISolutionProcessStartInfoPatcher processStartInfoPatcher,
            DefaultDotNetVsTestRunSettingsProvider runSettingsProvider,
            IDotNetVsTestCaseMap testCaseMap,
            MspecTestElementMapperFactory mapperFactory,
            INugetReferenceChecker nugetChecker,
            IUnitTestingSettings unitTestingSettings,
            ILogger logger)
            : base(lifetime, provider, sdkResolver, serverFactory, processStartInfoPatcher, runSettingsProvider, testCaseMap, mapperFactory, nugetChecker, unitTestingSettings, logger)
        {
        }

        public override bool IsSupported(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return base.IsSupported(project, targetFrameworkId) &&
                   (targetFrameworkId.IsNetCoreApp || targetFrameworkId.IsNetFramework);
        }

        protected override IEnumerable<string> NugetReferencesToCheck()
        {
            yield return "Microsoft.NET.Test.Sdk";
            yield return "Machine.Specifications.Runner.VisualStudio";
        }
    }
}
