using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class PsiFieldInfoAdapter : IFieldInfo
    {
        private readonly IField _field;

        public PsiFieldInfoAdapter(IField field)
        {
            _field = field;
        }

        public string DeclaringType => _field.GetContainingType()?.GetClrName().FullName;

        public string ShortName => _field.ShortName;

        public ITypeInfo FieldType
        {
            get
            {
                if (_field.Type is IDeclaredType type && type.IsResolved && type.IsValid())
                {
                    if (type.IsClassType())
                        return new PsiClassInfoAdapter(type.GetClassType());

                    if (type.IsDelegate())
                        return new PsiDelegateInfoAdapter(type.GetDelegateType(), type);
                }

                return UnknownTypeInfoAdapter.Default;
            }
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
        {
            return _field.GetAttributeInstances(new ClrTypeName(typeName), inherit)
                .Select(x => x.AsAttributeInfo());
        }
    }
}
