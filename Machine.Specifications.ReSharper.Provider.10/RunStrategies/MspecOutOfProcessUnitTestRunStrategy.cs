using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider.RunStrategies
{
    public class MspecOutOfProcessUnitTestRunStrategy : OutOfProcessUnitTestRunStrategy
    {
        public MspecOutOfProcessUnitTestRunStrategy()
            : base(new RemoteTaskRunnerInfo(MspecTaskRunner.RunnerId, typeof(MspecTaskRunner)))
        {
        }
    }
}
