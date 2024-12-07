using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.NuGet.Packaging;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.Util.Collections;

namespace Machine.Specifications.Runner.ReSharper;

[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public class MspecTestExplorerFromTestRunner(
    MspecTestProvider provider,
    ITestRunnerAgentManager agentManager,
    MspecTestRunnerOrchestrator adapter,
    NuGetInstalledPackageChecker installedPackageChecker)
    : UnitTestExplorerFrom.TestRunner(provider, agentManager, adapter, installedPackageChecker, 1.Minute());
