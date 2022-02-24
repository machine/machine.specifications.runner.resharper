using JetBrains.ReSharper.TestFramework;
using Machine.Specifications.Runner.ReSharper.Tests.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Source
{
    [MspecReferences]
    [TestNetFramework46]
    public class MspecSourceTests : UnitTestSourceTestBase
    {
        protected override string RelativeTestDataPath => "Exploration";

        [TestCase("SimpleSpec.cs")]
        [TestCase("IgnoredSpec.cs")]
        [TestCase("IgnoredContext.cs")]
        [TestCase("BehaviorSpecs.cs")]
        public void TestFile(string filename)
        {
            var path = GetTestDataFilePath2(filename);

            DoTestSolution(path.ToString());
        }
    }
}
