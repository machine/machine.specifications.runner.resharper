using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using Machine.Specifications.Runner.ReSharper.Runner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecTaskRunnerTestBase : UnitTestTaskRunnerTestBase
    {
        public override IUnitTestExplorerFromArtifacts MetadataExplorer
        {
            get
            {
                var runner = Solution.GetComponent<MspecTestExplorerFromTestRunner>();

                return Solution.GetComponent<MspecTestExplorerFromArtifacts>();
            }
        }

        protected override void ChangeSettingsForTest(IContextBoundSettingsStoreLive settingsStore)
        {
        }

        protected override RemoteTaskRunnerInfo GetRemoteTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(MspecTaskRunner.RunnerId, typeof(MspecTaskRunner));
        }
    }
}
