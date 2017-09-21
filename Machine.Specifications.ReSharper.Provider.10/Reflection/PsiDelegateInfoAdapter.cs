using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class PsiDelegateInfoAdapter : ITypeInfo
    {
        private readonly IDelegate _type;
        private readonly IDeclaredType _declaredType;

        public PsiDelegateInfoAdapter(IDelegate type, IDeclaredType declaredType)
        {
            _type = type;
            _declaredType = declaredType;
        }

        public string FullName => _type.GetClrName().FullName;

        public bool IsAbstract => _type.IsAbstract;

        public IEnumerable<IFieldInfo> GetFields()
        {
            return Enumerable.Empty<IFieldInfo>();
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
        {
            return _type.GetAttributeInstances(new ClrTypeName(typeName), inherit)
                .Select(x => x.AsAttributeInfo());
        }

        public IEnumerable<ITypeInfo> GetGenericArguments()
        {
            var substitution = _declaredType.GetSubstitution();

            return substitution.Domain
                .Select(x => substitution.Apply(x).GetScalarType())
                .Where(x => x != null)
                .Select(x => x.GetTypeElement())
                .OfType<IClass>()
                .Select(x => x.AsTypeInfo());
        }
    }
}