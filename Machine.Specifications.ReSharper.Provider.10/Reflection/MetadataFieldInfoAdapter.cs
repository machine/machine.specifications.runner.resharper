using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class MetadataFieldInfoAdapter : IFieldInfo
    {
        private readonly IMetadataField _field;

        public MetadataFieldInfoAdapter(IMetadataField field)
        {
            _field = field;
        }

        public string DeclaringType => _field.DeclaringType.FullyQualifiedName;

        public string ShortName => _field.Name;

        public ITypeInfo FieldType
        {
            get
            {
                if (_field.Type is IMetadataClassType classType)
                    return classType.Type.AsTypeInfo(classType);

                return UnknownTypeInfoAdapter.Default;
            }
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
        {
            return _field.GetCustomAttributes(typeName)
                .Select(x => x.AsAttributeInfo());
        }
    }
}