using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;

namespace Machine.Specifications.Runner.ReSharper.Reflection;

public class MetadataAttributeInfoAdapter(IMetadataCustomAttribute attribute) : IAttributeInfo
{
    public IEnumerable<string> GetParameters()
    {
        var arguments = attribute.ConstructorArguments
            .Where(x => !x.IsBadValue())
            .ToArray();

        var types = arguments
            .Select(x => x.Value)
            .OfType<IMetadataClassType>()
            .Select(x => new ClrTypeName(x.Type.FullyQualifiedName))
            .Select(x => x.ShortName);

        var values = arguments
            .Select(x => x.Value)
            .OfType<string>();

        var arrayValues = arguments
            .Where(x => x.ValuesArray != null)
            .SelectMany(x => x.ValuesArray)
            .Select(x => x.Value)
            .OfType<string>();

        return types
            .Concat(values)
            .Concat(arrayValues);
    }
}
