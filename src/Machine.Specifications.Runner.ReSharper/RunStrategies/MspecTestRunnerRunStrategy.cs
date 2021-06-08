using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Access;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DotNetCore;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.DataCollection;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper.RunStrategies
{
    [SolutionComponent]
    public class MspecTestRunnerRunStrategy : TestRunnerRunStrategy
    {
        public MspecTestRunnerRunStrategy(
            IDataCollectorFactory dataCollectorFactory,
            ITestRunnerAgentManager agentManager,
            ITestRunnerHostSource testRunnerHostSource,
            MspecTestRunnerOrchestrator adapter,
            IUnitTestProjectArtifactResolver artifactResolver,
            DotNetCoreLaunchSettingsJsonProfileProvider launchSettingsProvider)
            : base(dataCollectorFactory, agentManager, testRunnerHostSource, adapter, artifactResolver, launchSettingsProvider)
        {
        }

        protected override TestRunnerRuntimeEnvironment GetRuntimeEnvironmentCore(IUnitTestLaunch launch, IProject project,
            TargetFrameworkId targetFrameworkId)
        {
            var e = base.GetRuntimeEnvironmentCore(launch, project, targetFrameworkId);

            return new MspecEnvironemnt(e.Project, e.TargetPlatform, e.TargetFrameworkId, e.RunAsTargetFrameworkId, e.PlatformMonoPreference, e.IsUnmanaged);
        }

        private class MspecEnvironemnt : TestRunnerRuntimeEnvironment
        {
            public MspecEnvironemnt(
                IProject project,
                TargetPlatform targetPlatform,
                TargetFrameworkId targetFrameworkId,
                TargetFrameworkId runAsTargetFrameworkId,
                PlatformMonoPreference platformMonoPreference,
                bool unmanaged)
                : base(new MspecTestRunnerHost(), project, targetPlatform, targetFrameworkId, runAsTargetFrameworkId, platformMonoPreference, unmanaged)
            {
            }
        }

        private class MspecTestRunnerHost : DefaultTestRunnerHost
        {
            public override IEnumerable<IMessageHandlerMarker> GetMessageHandlers(ITestRunnerContext ctx)
            {
                var h = base.GetMessageHandlers(ctx).ToArray();

                return h;
            }
        }
    }
}
