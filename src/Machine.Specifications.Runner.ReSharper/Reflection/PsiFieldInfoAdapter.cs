using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.Runner.ReSharper.Reflection;

public class PsiFieldInfoAdapter(IField field) : IFieldInfo
{
    public string DeclaringType => field.ContainingType!.GetClrName().FullName;

    public string ShortName => field.ShortName;

    public ITypeInfo FieldType
    {
        get
        {
            if (field.Type is IDeclaredType {IsResolved: true} type)
            {
                return type.GetTypeElement()!.AsTypeInfo(type);
            }

            return UnknownTypeInfoAdapter.Default;
        }
    }

    public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
    {
        return field.GetAttributeInstances(new ClrTypeName(typeName), inherit)
            .Select(x => x.AsAttributeInfo());
    }
}
