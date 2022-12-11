using JetBrains.ProjectModel;
using JetBrains.ProjectModel.NuGet.Packaging;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.Util.Collections;

namespace Machine.Specifications.Runner.ReSharper;

[SolutionComponent]
public class MspecTestExplorerFromTestRunner : UnitTestExplorerFrom.TestRunner
{
    public MspecTestExplorerFromTestRunner(
        MspecTestProvider provider,
        ITestRunnerAgentManager agentManager,
        MspecTestRunnerOrchestrator adapter,
        NuGetInstalledPackageChecker installedPackageChecker)
        : base(provider, agentManager, adapter, installedPackageChecker, 1.Minute())
    {
    }
}
