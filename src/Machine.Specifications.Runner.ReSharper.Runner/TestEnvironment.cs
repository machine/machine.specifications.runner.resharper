using System.IO;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class TestEnvironment
    {
        public TestEnvironment(string assemblyLocation, bool shouldShadowCopy)
        {
            AssemblyFolder = GetAssemblyFolder(TaskExecutor.Configuration, assemblyLocation);
            AssemblyPath = Path.Combine(AssemblyFolder, GetFileName(assemblyLocation));
            ShouldShadowCopy = shouldShadowCopy;
            ShadowCopyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        public string AssemblyFolder { get; }

        public string AssemblyPath { get; }

        public bool ShouldShadowCopy { get; }

        public string ShadowCopyPath { get; }

        private string GetAssemblyFolder(TaskExecutorConfiguration config, string assemblyLocation)
        {
            if (!string.IsNullOrEmpty(config?.AssemblyFolder))
            {
                return config.AssemblyFolder;
            }

            return Path.GetDirectoryName(assemblyLocation);
        }

        private string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}
