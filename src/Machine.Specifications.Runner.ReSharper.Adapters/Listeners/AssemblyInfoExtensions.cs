using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    internal static class AssemblyInfoExtensions
    {
        public static TestAssemblyInfo ToTestAssembly(this AssemblyInfo assembly)
        {
            return new TestAssemblyInfo(assembly.Location);
        }
    }
}
