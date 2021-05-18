using System.Collections.Generic;

namespace Machine.Specifications.Runner.ReSharper.Reflection
{
    public interface IFieldInfo
    {
        string? DeclaringType { get; }

        string ShortName { get; }

        ITypeInfo FieldType { get; }

        IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit);
    }
}
