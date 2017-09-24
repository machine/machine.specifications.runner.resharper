using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class MetadataTypeInfoAdapter : ITypeInfo
    {
        private readonly IMetadataTypeInfo _type;
        private readonly IMetadataClassType _classType;

        public MetadataTypeInfoAdapter(IMetadataTypeInfo type, IMetadataClassType classType = null)
        {
            _type = type;
            _classType = classType;
        }

        public string ShortName => _type.Name;

        public string FullName => _type.FullyQualifiedName;

        public bool IsAbstract => _type.IsAbstract;

        public ITypeInfo GetContainingType()
        {
            return _type.DeclaringType?.AsTypeInfo();
        }

        public IEnumerable<IFieldInfo> GetFields()
        {
            return _type.GetFields()
                .Where(x => !x.IsStatic)
                .Select(x => x.AsFieldInfo());
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
        {
            var attributes = _type.GetCustomAttributes(typeName)
                .Select(x => x.AsAttributeInfo());

            if (!inherit)
                return attributes;

            var baseAttributes = GetBaseTypes()
                .SelectMany(x => x.GetCustomAttributes(typeName, false));

            return attributes.Concat(baseAttributes);
        }

        public IEnumerable<ITypeInfo> GetGenericArguments()
        {
            var arguments = _classType?.Arguments ?? _type.ToClassType()?.Arguments;

            return arguments?
                .OfType<IMetadataClassType>()
                .Select(x => x.Type.AsTypeInfo());
        }

        private IEnumerable<ITypeInfo> GetBaseTypes()
        {
            var type = _type;

            while (type.Base != null)
            {
                yield return type.Base.Type.AsTypeInfo();

                type = type.Base.Type;
            }
        }
    }
}