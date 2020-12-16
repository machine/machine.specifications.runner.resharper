using JetBrains.Application.Infra;
using JetBrains.Application.platforms;
using JetBrains.Application.Processes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.NuGet.Packaging;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util.Collections;
using IUnitTestAgentManager = JetBrains.ReSharper.UnitTestFramework.TestRunner.IUnitTestAgentManager;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestExplorerFromTestRunner : UnitTestExplorerFrom.TestRunner
    {
        public MspecTestExplorerFromTestRunner(
            MspecTestProvider provider,
            IUnitTestAgentManager agentManager,
            MspecTestRunnerOrchestrator adapter,
            IPlatformManager platformManager,
            IAssemblyInfoDatabase assemblyInfoDatabase,
            ISolutionProcessStartInfoPatcher processStartInfoPatcher,
            NuGetInstalledPackageChecker installedPackageChecker)
            : base(provider, agentManager, adapter, platformManager, assemblyInfoDatabase, processStartInfoPatcher, installedPackageChecker, 30.Minutes())
        {
        }
    }
}
