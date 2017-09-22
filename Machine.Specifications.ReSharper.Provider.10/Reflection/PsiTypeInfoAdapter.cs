using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class PsiTypeInfoAdapter : ITypeInfo
    {
        private readonly ITypeElement _type;
        private readonly IDeclaredType _declaredType;

        public PsiTypeInfoAdapter(ITypeElement type, IDeclaredType declaredType = null)
        {
            _type = type;
            _declaredType = declaredType;
        }

        public string FullName => _type.GetClrName().FullName;

        public bool IsAbstract => _type is IModifiersOwner owner && owner.IsAbstract;

        public IEnumerable<IFieldInfo> GetFields()
        {
            if (!(_type is IClass type))
                return Enumerable.Empty<IFieldInfo>();

            return type.Fields
                .Where(x => !x.IsStatic)
                .Select(x => x.AsFieldInfo());
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
        {
            return _type.GetAttributeInstances(new ClrTypeName(typeName), inherit)
                .Select(x => x.AsAttributeInfo());
        }

        public IEnumerable<ITypeInfo> GetGenericArguments()
        {
            if (_declaredType != null)
            {
                var substitution = _declaredType.GetSubstitution();

                return substitution.Domain
                    .Select(x => substitution.Apply(x).GetScalarType())
                    .Where(x => x != null)
                    .Select(x => x.GetTypeElement())
                    .OfType<IClass>()
                    .Select(x => x.AsTypeInfo());
            }

            if (_type is ITypeParametersOwner owner)
                return owner.TypeParameters.Select(x => x.AsTypeInfo());

            return Enumerable.Empty<ITypeInfo>();
        }
    }
}