using System;
using System.Reflection;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution
{
    [SolutionComponent]
    public class AssemblyResolver : IAssemblyResolver
    {
        public Type GetType(string fullyQualifiedName)
        {
            return Type.GetType(fullyQualifiedName)!;
        }

        public Assembly LoadFrom(string assemblyFile)
        {
            return Assembly.LoadFrom(assemblyFile);
        }

        public IDisposable RegisterAssembly(AssemblyName asmName, string pathToAssembly)
        {
            return Disposable.Empty;
        }

        public IDisposable RegisterPath(string path)
        {
            return Disposable.Empty;
        }

        public IDisposable ResolveRelativeTo(string path)
        {
            return Disposable.Empty;
        }
    }
}
