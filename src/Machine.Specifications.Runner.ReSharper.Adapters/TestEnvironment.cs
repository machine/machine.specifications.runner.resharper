using System.IO;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class TestEnvironment
    {
        public TestEnvironment(string assemblyLocation, bool shouldShadowCopy)
        {
            AssemblyFolder = GetAssemblyFolder(assemblyLocation);
            AssemblyPath = Path.Combine(AssemblyFolder, GetFileName(assemblyLocation));
            ShouldShadowCopy = shouldShadowCopy;
            ShadowCopyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        public string AssemblyFolder { get; }

        public string AssemblyPath { get; }

        public bool ShouldShadowCopy { get; }

        public string ShadowCopyPath { get; }

        private string GetAssemblyFolder(string assemblyLocation)
        {
            return Path.GetDirectoryName(assemblyLocation);
        }

        private string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}
