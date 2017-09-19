using System.Collections.Generic;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public interface ITypeInfo
    {
        bool IsAbstract { get; }

        string FullyQualifiedName { get; }

        IEnumerable<IFieldInfo> GetFields();
        
        IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName);

        IEnumerable<ITypeInfo> GetGenericArguments();
    }
}
