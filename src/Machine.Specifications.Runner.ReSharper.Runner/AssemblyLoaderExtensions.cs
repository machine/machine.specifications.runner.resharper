using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public static class AssemblyLoaderExtensions
    {
        public static AssemblyLoader WithDefaultTypes(this AssemblyLoader loader)
        {
            loader.RegisterAssemblyOf<AssemblyLoader>();
            loader.RegisterAssemblyOf<TestRunner>();

            return loader;
        }
    }
}
