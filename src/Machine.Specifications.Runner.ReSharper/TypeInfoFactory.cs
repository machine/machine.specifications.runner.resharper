using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper;

public static class TypeInfoFactory
{
    public static TypeInfo Create(string typeName, string assemblyLocation)
    {
        return new TypeInfo(typeName, assemblyLocation);
    }
}
