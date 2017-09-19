using System.Collections.Generic;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public interface IFieldInfo
    {
        ITypeInfo FieldType { get; }

        string DeclaringType { get; }

        string ShortName { get; }

        IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName);
    }
}
