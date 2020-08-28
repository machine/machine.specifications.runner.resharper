using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;

namespace Machine.Specifications.Runner.ReSharper.Reflection
{
    public class MetadataFieldInfoAdapter : IFieldInfo
    {
        private readonly IMetadataField field;

        public MetadataFieldInfoAdapter(IMetadataField field)
        {
            this.field = field;
        }

        public string DeclaringType => field.DeclaringType.FullyQualifiedName;

        public string ShortName => field.Name;

        public ITypeInfo FieldType
        {
            get
            {
                if (field.Type is IMetadataClassType classType)
                {
                    return classType.Type.AsTypeInfo(classType);
                }

                return UnknownTypeInfoAdapter.Default;
            }
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
        {
            return field.GetCustomAttributes(typeName)
                .Select(x => x.AsAttributeInfo());
        }
    }
}
