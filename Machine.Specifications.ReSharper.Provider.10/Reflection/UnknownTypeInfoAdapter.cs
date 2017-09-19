using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class UnknownTypeInfoAdapter : ITypeInfo
    {
        public static readonly UnknownTypeInfoAdapter Default = new UnknownTypeInfoAdapter();

        public bool IsAbstract => false;

        public string FullyQualifiedName => string.Empty;

        public IEnumerable<IFieldInfo> GetFields()
        {
            return Enumerable.Empty<IFieldInfo>();
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName)
        {
            return Enumerable.Empty<IAttributeInfo>();
        }

        public IEnumerable<ITypeInfo> GetGenericArguments()
        {
            return Enumerable.Empty<ITypeInfo>();
        }
    }
}
