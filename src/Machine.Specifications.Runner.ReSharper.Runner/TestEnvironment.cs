using System;
using System.IO;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class TestEnvironment
    {
        public TestEnvironment(MspecAssemblyTask assemblyTask, IShadowCopyCookie shadowCopyCookie)
        {
            var assembly = TaskExecutor.Configuration.GetAssemblyLocation(assemblyTask.AssemblyLocation);

            AssemblyPath = shadowCopyCookie.Rewrite(assembly);
            AssemblyFolder = Path.GetDirectoryName(AssemblyPath);

            if (string.IsNullOrEmpty(AssemblyFolder))
            {
                throw new InvalidOperationException("Tests cannot be run from the root folder");
            }

            Environment.CurrentDirectory = AssemblyFolder;
        }

        public string AssemblyPath { get; }

        public string AssemblyFolder { get; }
    }
}
