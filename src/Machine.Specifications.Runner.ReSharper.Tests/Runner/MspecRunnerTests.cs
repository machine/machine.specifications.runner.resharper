using System.Linq;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Runner
{
    [MspecReferences]
    [TestNetFramework46]
    public class MspecRunnerTests : MspecTaskRunnerTestBase
    {
        protected override string RelativeTestDataPath => "Runner";

        [ExcludeMsCorLib]
        //[TestCase("SimpleSpec.cs")]
        //[TestCase("FailingSpec.cs")]
        public void TestFile(string filename)
        {
            var path = GetTestDataFilePath2(filename);

            var references = GetReferencedAssemblies(GetTargetFrameworkId()).ToArray();
            var assembly = CodeCompiler.CompileSource(path, references);

            DoTestSolution(assembly);
        }
    }
}
