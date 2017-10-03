using JetBrains.Metadata.Reader.API;
using Machine.Specifications.ReSharperProvider.Reflection;

namespace Machine.Specifications.ReSharperProvider
{
    public static class MetadataExtensions
    {
        public static ITypeInfo AsTypeInfo(this IMetadataTypeInfo type, IMetadataClassType classType = null)
        {
            return new MetadataTypeInfoAdapter(type, classType);
        }

        public static IAttributeInfo AsAttributeInfo(this IMetadataCustomAttribute attribute)
        {
            return new MetadataAttributeInfoAdapter(attribute);
        }

        public static IFieldInfo AsFieldInfo(this IMetadataField field)
        {
            return new MetadataFieldInfoAdapter(field);
        }
    }
}