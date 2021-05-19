using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Metadata
{
    [MspecReferences]
    [TestNetFramework46]
    public class MspecMetadataTests : MspecMetadataTestBase
    {
        protected override string RelativeTestDataPath => "Exploration";

        [ExcludeMsCorLib]
        [TestCase("SimpleSpec.cs")]
        [TestCase("IgnoredSpec.cs")]
        [TestCase("IgnoredContext.cs")]
        public void TestFile(string filename)
        {
            var path = GetTestDataFilePath2(filename);
            var dll = GetDll(path);

            DoTestSolution(dll);
        }

        private string GetDll(FileSystemPath source)
        {
            var assembly = source.ChangeExtension("dll");

            if (assembly.ExistsFile && source.FileModificationTimeUtc <= assembly.FileModificationTimeUtc)
            {
                return assembly.Name;
            }

            var references = GetReferencedAssemblies(GetTargetFrameworkId())
                .Where(Path.IsPathRooted)
                .ToArray();

            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.AddRange(references);
            parameters.OutputAssembly = assembly.ToString();

            var provider = new CSharpCodeProvider();
            var result = provider.CompileAssemblyFromFile(parameters, source.ToString());

            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors.ToStringWithCount());
            }

            return assembly.Name;
        }
    }
}
