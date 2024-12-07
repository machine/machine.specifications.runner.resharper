using System.Collections.Generic;

namespace Machine.Specifications.Runner.ReSharper.Reflection;

public class UnknownTypeInfoAdapter : ITypeInfo
{
    public static readonly UnknownTypeInfoAdapter Default = new();

    public string FullyQualifiedName => string.Empty;

    public bool IsAbstract => false;

    public ITypeInfo? GetContainingType()
    {
        return null;
    }

    public IEnumerable<IFieldInfo> GetFields()
    {
        return [];
    }

    public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
    {
        return [];
    }

    public IEnumerable<ITypeInfo> GetGenericArguments()
    {
        return [];
    }
}
