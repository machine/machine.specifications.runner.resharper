using System.Threading;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Runner
{
    [TestFixture]
    public class MspecRunnerTests : MspecTaskRunnerTestBase
    {
        protected override string RelativeTestDataPath => "Runner";

        [Test]
        public void SimpleSpec()
        {
            var agent = Solution.GetComponent<ITestRunnerAgentManager>();

            //var observer = new TestUnitTestElementObserver();

            var runner = Solution.GetComponent<MspecTestExplorerFromTestRunner>();
            //var result = runner.ProcessArtifact(observer, CancellationToken.None).Result;

            DoOneTest("SimpleSpec");
        }
    }
}
