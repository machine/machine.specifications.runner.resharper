using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using JetBrains.Util;
using Microsoft.CSharp;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework;

public static class CodeCompiler
{
    public static string CompileSource(FileSystemPath source, string[] references)
    {
        var assembly = source.ChangeExtension("dll");

        if (assembly.ExistsFile && source.FileModificationTimeUtc <= assembly.FileModificationTimeUtc)
        {
            return assembly.Name;
        }

        var rootedReferences = references
            .Where(Path.IsPathRooted)
            .ToArray();

        var parameters = new CompilerParameters();
        parameters.ReferencedAssemblies.AddRange(rootedReferences);
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
