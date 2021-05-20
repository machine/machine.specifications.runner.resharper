using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Runner
{
    [MspecReferences]
    [TestNetFramework46]
    public class MspecRunnerTests : MspecTaskRunnerTestBase
    {
        protected override string RelativeTestDataPath => "Runner";

        [Test]
        [ExcludeMsCorLib]
        public void SimpleSpec()
        {
            DoOneTest("SimpleSpec");
        }
    }
}
