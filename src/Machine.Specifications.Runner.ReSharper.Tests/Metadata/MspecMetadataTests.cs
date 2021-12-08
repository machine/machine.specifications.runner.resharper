using System.Linq;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Metadata
{
    [MspecReferences]
    [TestNetFramework46]
    public class MspecMetadataTests : UnitTestMetadataTestBase
    {
        protected override string RelativeTestDataPath => "Exploration";

        [ExcludeMsCorLib]
        [TestCase("SimpleSpec.cs")]
        [TestCase("IgnoredSpec.cs")]
        [TestCase("IgnoredContext.cs")]
        public void TestFile(string filename)
        {
            var path = GetTestDataFilePath2(filename);

            var references = GetReferencedAssemblies(GetTargetFrameworkId()).ToArray();
            var assembly = CodeCompiler.CompileSource(path, references);

            DoTestSolution(assembly);
        }
    }
}
