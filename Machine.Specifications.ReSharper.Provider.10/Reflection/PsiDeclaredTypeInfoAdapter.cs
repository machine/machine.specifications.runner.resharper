using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class PsiDeclaredTypeInfoAdapter : ITypeInfo
    {
        private readonly IDeclaredType _type;

        public PsiDeclaredTypeInfoAdapter(IDeclaredType type) 
        {
            _type = type;
        }

        public bool IsAbstract => false;

        public string FullyQualifiedName => _type.GetClrName().FullName;

        public IEnumerable<IFieldInfo> GetFields()
        {
            return _type.GetClassType()?.Fields
                .Select(x => x.AsFieldInfo());
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName)
        {
            return _type.GetTypeElement()?
                .GetAttributeInstances(new ClrTypeName(typeName), false)
                .Select(x => x.AsAttributeInfo());
        }

        public IEnumerable<ITypeInfo> GetGenericArguments()
        {
            var substitution = _type.GetSubstitution();

            return substitution.Domain
                .Select(x => substitution.Apply(x).GetScalarType()?.GetTypeElement())
                .OfType<IClass>()
                .Select(x => x.AsTypeInfo());
        }
    }
}
