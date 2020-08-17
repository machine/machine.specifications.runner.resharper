using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper.RunStrategies
{
    [SolutionComponent]
    public class MspecOutOfProcessUnitTestRunStrategy : TaskRunnerOutOfProcessUnitTestRunStrategy
    {
        public MspecOutOfProcessUnitTestRunStrategy(IUnitTestAgentManager agentManager, IUnitTestResultManager resultManager)
            : base(agentManager, resultManager, new RemoteTaskRunnerInfo(MspecTaskRunner.RunnerId, typeof(MspecTaskRunner)))
        {
        }

        protected override TaskRunnerRuntimeEnvironment GetRuntimeEnvironmentCore(
            IUnitTestLaunch launch,
            IProject project,
            TargetFrameworkId targetFrameworkId)
        {
            var environment = base.GetRuntimeEnvironmentCore(launch, project, targetFrameworkId);

            return new MspecTaskRunnerRuntimeEnvironment(
                project,
                environment.TargetPlatform,
                environment.TargetFrameworkId,
                environment.IsUnmanaged,
                environment.PlatformMonoPreference);
        }
    }
}
