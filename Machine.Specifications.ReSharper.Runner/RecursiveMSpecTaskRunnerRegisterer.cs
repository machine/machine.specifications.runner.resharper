using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner
{
    [SolutionComponent]
    public class RecursiveMSpecTaskRunnerRegisterer
    {
        public RecursiveMSpecTaskRunnerRegisterer(UnitTestingAssemblyLoader loader)
        {
            loader.RegisterAssembly(typeof(RecursiveMSpecTaskRunner).Assembly);
        }
    }
}