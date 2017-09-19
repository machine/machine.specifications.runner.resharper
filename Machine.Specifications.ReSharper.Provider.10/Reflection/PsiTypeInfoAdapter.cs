using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class PsiTypeInfoAdapter : ITypeInfo
    {
        private readonly IClass _type;

        public PsiTypeInfoAdapter(IClass type)
        {
            _type = type;
        }

        public bool IsAbstract => _type.IsAbstract;

        public string FullyQualifiedName => _type.GetClrName().FullName;

        public IEnumerable<IFieldInfo> GetFields()
        {
            return _type.Fields
                .Where(x => !x.IsStatic)
                .Where(x => x.Type is IDeclaredType)
                .Where(x => x.Type.IsResolved && x.Type.IsValid())
                .Select(x => x.AsFieldInfo());
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName)
        {
            return _type.GetAttributeInstances(true)
                .Where(x => x.GetClrName().FullName == typeName)
                .Select(x => x.AsAttributeInfo());
        }

        public virtual IEnumerable<ITypeInfo> GetGenericArguments()
        {
            return Enumerable.Empty<ITypeInfo>();
        }
    }
}
