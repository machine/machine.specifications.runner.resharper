using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class PsiClassInfoAdapter : ITypeInfo
    {
        private readonly IClass _type;

        public PsiClassInfoAdapter(IClass type)
        {
            _type = type;
        }

        public string FullName => _type.GetClrName().FullName;

        public bool IsAbstract => _type.IsAbstract;

        public IEnumerable<IFieldInfo> GetFields()
        {
            return _type.Fields
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
            return Enumerable.Empty<ITypeInfo>();
        }
    }
}