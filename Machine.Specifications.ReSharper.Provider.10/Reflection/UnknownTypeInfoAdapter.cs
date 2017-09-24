using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class UnknownTypeInfoAdapter : ITypeInfo
    {
        public static readonly UnknownTypeInfoAdapter Default = new UnknownTypeInfoAdapter();

        public string ShortName => string.Empty;

        public string FullName => string.Empty;

        public bool IsAbstract => false;

        public ITypeInfo GetContainingType()
        {
            return null;
        }

        public IEnumerable<IFieldInfo> GetFields()
        {
            return Enumerable.Empty<IFieldInfo>();
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
        {
            return Enumerable.Empty<IAttributeInfo>();
        }

        public IEnumerable<ITypeInfo> GetGenericArguments()
        {
            return Enumerable.Empty<ITypeInfo>();
        }
    }
}
