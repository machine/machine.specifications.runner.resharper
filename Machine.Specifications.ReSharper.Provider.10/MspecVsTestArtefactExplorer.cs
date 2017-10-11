using System.Collections.Generic;
using JetBrains.Application.ProcessRunner;
using JetBrains.DataFlow;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.Channel;
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
            IDotNetCoreUnitTestServerFactory serverFactory, 
            IProcessRunnerManager processRunnerManager, 
            IDotNetVsTestCaseMap testCaseMap, 
            MspecTestElementMapperFactory mapperFactory, 
            INugetReferenceChecker nugetChecker, 
            ILogger logger) 
            : base(lifetime, provider, sdkResolver, serverFactory, processRunnerManager, testCaseMap, mapperFactory, nugetChecker, logger)
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
