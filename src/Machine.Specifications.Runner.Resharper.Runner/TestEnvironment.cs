using System.IO;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner
{
    public class TestEnvironment
    {
        public TestEnvironment(MspecTestAssemblyTask assemblyTask)
        {
            AssemblyFolder = GetAssemblyFolder(TaskExecutor.Configuration, assemblyTask);
            AssemblyPath = Path.Combine(AssemblyFolder, GetFileName(assemblyTask.AssemblyLocation));
            ShouldShadowCopy = TaskExecutor.Configuration.ShadowCopy != ShadowCopyOption.None;
            ShadowCopyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        public string AssemblyFolder { get; }

        public string AssemblyPath { get; }

        public bool ShouldShadowCopy { get; }

        public string ShadowCopyPath { get; }

        private string GetAssemblyFolder(TaskExecutorConfiguration config, MspecTestAssemblyTask assemblyTask)
        {
            if (!string.IsNullOrEmpty(config.AssemblyFolder))
                return config.AssemblyFolder;

            return Path.GetDirectoryName(assemblyTask.AssemblyLocation);
        }

        private string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}
