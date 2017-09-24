using System.Collections.Generic;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public interface ITypeInfo
    {
        string ShortName { get; }

        string FullName { get; }

        bool IsAbstract { get; }

        ITypeInfo GetContainingType();

        IEnumerable<IFieldInfo> GetFields();

        IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit);

        IEnumerable<ITypeInfo> GetGenericArguments();
    }
}
