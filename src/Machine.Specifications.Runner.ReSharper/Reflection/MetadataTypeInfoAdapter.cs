﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;

namespace Machine.Specifications.Runner.ReSharper.Reflection;

public class MetadataTypeInfoAdapter(IMetadataTypeInfo type, IMetadataClassType? classType = null)
    : ITypeInfo
{
    public string FullyQualifiedName => type.FullyQualifiedName;

    public bool IsAbstract => type.IsAbstract;

    public ITypeInfo? GetContainingType()
    {
        return type.DeclaringType?.AsTypeInfo();
    }

    public IEnumerable<IFieldInfo> GetFields()
    {
        return type.GetFields()
            .Where(x => !x.IsStatic)
            .Select(x => x.AsFieldInfo());
    }

    public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName, bool inherit)
    {
        var attributes = type.GetCustomAttributes(typeName)
            .Select(x => x.AsAttributeInfo());

        if (!inherit)
        {
            return attributes;
        }

        var baseAttributes = GetBaseTypes()
            .SelectMany(x => x.GetCustomAttributes(typeName, false));

        return attributes.Concat(baseAttributes);
    }

    public IEnumerable<ITypeInfo> GetGenericArguments()
    {
        if (classType == null)
        {
            return type.TypeParameters.Select(_ => UnknownTypeInfoAdapter.Default);
        }

        return classType.Arguments
            .OfType<IMetadataClassType>()
            .Select(x => x.Type.AsTypeInfo());
    }

    private IEnumerable<ITypeInfo> GetBaseTypes()
    {
        var current = type;

        while (current.Base != null)
        {
            yield return current.Base.Type.AsTypeInfo();

            current = current.Base.Type;
        }
    }
}
