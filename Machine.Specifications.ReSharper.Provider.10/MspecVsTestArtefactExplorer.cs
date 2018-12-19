using System.Collections.Generic;
using JetBrains.Application.Processes;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.NuGet.Packaging;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Channel.Json;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.DotNetVsTest;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

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
            NuGetInstalledPackageChecker nugetChecker,
            IUnitTestingSettings unitTestingSettings,
            ILogger logger)
            : base(lifetime, provider, sdkResolver, serverFactory, processStartInfoPatcher, runSettingsProvider, testCaseMap, mapperFactory, nugetChecker, unitTestingSettings, logger)
        {
        }

        public override PertinenceResult IsSupported(IProject project, TargetFrameworkId targetFrameworkId)
        {
            var result = base.IsSupported(project, targetFrameworkId);

            if (result != PertinenceResult.Yes)
                return result;

            return targetFrameworkId.IsNetFramework ? PertinenceResult.No : PertinenceResult.Yes;
        }

        protected override IEnumerable<string> RequiredNuGetDependencies()
        {
            yield return "Microsoft.NET.Test.Sdk";
            yield return "Machine.Specifications.Runner.VisualStudio";
        }
    }
}
