using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider.RunStrategies
{
    [SolutionComponent]
    public class MspecOutOfProcessUnitTestRunStrategy : TaskRunnerOutOfProcessUnitTestRunStrategy
    {
        public MspecOutOfProcessUnitTestRunStrategy(IUnitTestAgentManager agentManager, IUnitTestResultManager resultManager)
            : base(agentManager, resultManager, new RemoteTaskRunnerInfo(MspecTaskRunner.RunnerId, typeof(MspecTaskRunner)))
        {
        }
    }
}
