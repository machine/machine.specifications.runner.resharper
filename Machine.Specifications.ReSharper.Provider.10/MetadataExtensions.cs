using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider
{
    internal static class MetadataExtensions
    {
        public static bool IsContext(this IMetadataTypeInfo type)
        {
            return !type.IsAbstract &&
                   !type.IsStruct() &&
                   type.GenericParameters.Length == 0 &&
                   !type.HasCustomAttribute(FullNames.BehaviorsAttribute) &&
                   (type.GetSpecifications().Any() || type.GetBehaviors().Any());
        }

        private static bool IsStruct(this IMetadataTypeInfo type)
        {
            if (type.Base != null)
                return type.Base.Type.FullyQualifiedName == typeof(ValueType).FullName;

            return false;
        }

        public static IEnumerable<IMetadataField> GetSpecifications(this IMetadataTypeInfo type)
        {
            return type.GetInstanceFieldsOfType(FullNames.AssertDelegateAttribute);
        }

        public static IEnumerable<IMetadataField> GetBehaviors(this IMetadataTypeInfo type)
        {
            IEnumerable<IMetadataField> behaviorFields = type.GetInstanceFieldsOfType(FullNames.BehaviorDelegateAttribute);

            foreach (IMetadataField field in behaviorFields)
            {
                if (field.GetFirstGenericArgument().HasCustomAttribute(FullNames.BehaviorsAttribute))
                    yield return field;
            }
        }

        private static IEnumerable<IMetadataField> GetInstanceFieldsOfType(this IMetadataTypeInfo type, string fullyQualifiedName)
        {
            var metadataFields = type.GetInstanceFields();
            var fields = metadataFields.Where(x => x.Type is IMetadataClassType);

            return fields.Where(x => (((IMetadataClassType)x.Type).Type.HasCustomAttribute(fullyQualifiedName)));
        }

        public static string GetSubjectString(this IMetadataEntity type)
        {
            var attributes = GetSubjectAttributes(type);

            if (attributes.Count != 1)
            {
                var asMember = type as IMetadataTypeMember;

                if (asMember != null && asMember.DeclaringType != null)
                    return asMember.DeclaringType.GetSubjectString();

                return null;
            }

            IMetadataCustomAttribute attribute = attributes.First();
            string[] parameterNames = attribute.ConstructorArguments.Select(GetParameterName).ToArray();

            return string.Join(" ", parameterNames);
        }

        private static IList<IMetadataCustomAttribute> GetSubjectAttributes(IMetadataEntity metadataEntity)
        {
            return metadataEntity.AndAllBaseTypes()
                .SelectMany(x => x.GetCustomAttributes(FullNames.SubjectAttribute))
                .ToList();
        }

        public static ICollection<string> GetTags(this IMetadataEntity type)
        {
            return type.AndAllBaseTypes()
                .SelectMany(x => x.GetCustomAttributes(FullNames.TagsAttribute))
                .Select(x => x.ConstructorArguments)
                .Flatten(tag => tag.FirstOrDefault().Value as string,
                    tag => tag.Skip(1).FirstOrDefault().ValuesArray.Select(v => v.Value as string))
                .Distinct()
                .ToList();
        }

        private static IEnumerable<IMetadataTypeInfo> AndAllBaseTypes(this IMetadataEntity type)
        {
            IMetadataTypeInfo GetTypeFromAttributeConstructor()
            {
                var attribute = type as IMetadataCustomAttribute;

                return attribute?.UsedConstructor.DeclaringType;
            }

            var typeInfo = type as IMetadataTypeInfo;

            if (typeInfo == null)
            {
                // type might be an attribute - which cannot be cast as IMetadataTypeInfo.
                typeInfo = GetTypeFromAttributeConstructor();

                if (typeInfo == null)
                {
                    // No idea how the get the type of the IMetadataEntity.
                    yield break;
                }
            }

            yield return typeInfo;

            while (typeInfo.Base != null)
            {
                yield return typeInfo.Base.Type;

                typeInfo = typeInfo.Base.Type;
            }
        }

        public static IMetadataTypeInfo GetFirstGenericArgument(this IMetadataField genericField)
        {
            var genericArgument = ((IMetadataClassType)genericField.Type).Arguments.First();

            return ((IMetadataClassType)genericArgument).Type;
        }

        public static IMetadataClassType FirstGenericArgumentClass(this IMetadataField genericField)
        {
            var genericArgument = ((IMetadataClassType)genericField.Type).Arguments.First();

            return genericArgument as IMetadataClassType;
        }

        public static bool IsIgnored(this IMetadataEntity type)
        {
            return type.HasCustomAttribute(FullNames.IgnoreAttribute);
        }

        private static string GetParameterName(MetadataAttributeValue x)
        {
            var typeArgument = x.Value as IMetadataClassType;

            if (typeArgument != null)
                return new ClrTypeName(typeArgument.Type.FullyQualifiedName).ShortName;

            return (string)x.Value;
        }

        private static IEnumerable<TResult> Flatten<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> singleResultSelector, Func<TSource, IEnumerable<TResult>> collectionResultSelector)
        {
            foreach (var s in source)
            {
                yield return singleResultSelector(s);
                var resultSelector = collectionResultSelector(s);

                if (resultSelector == null)
                    yield break;

                foreach (var coll in collectionResultSelector(s))
                    yield return coll;
            }
        }

        private static IEnumerable<IMetadataField> GetInstanceFields(this IMetadataTypeInfo type)
        {
            return type.GetFields().Where(field => !field.IsStatic);
        }

        public static string FullyQualifiedName(this IMetadataClassType classType)
        {
            return FullyQualifiedName(classType, false);
        }

        private static string FullyQualifiedName(this IMetadataClassType classType, bool appendAssembly)
        {
            var fullyQualifiedName = new StringBuilder();

            fullyQualifiedName.Append(classType.Type.FullyQualifiedName);

            if (classType.Arguments.Length > 0)
            {
                fullyQualifiedName.Append("[");
                fullyQualifiedName.Append(
                  String.Join(",",
                              classType.Arguments
                                .Select(x => x as IMetadataClassType)
                                .Where(x => x != null)
                                .Select(x => "[" + x.FullyQualifiedName(true) + "]")
                                .ToArray()));
                fullyQualifiedName.Append("]");
            }

            if (appendAssembly)
                fullyQualifiedName.Append(classType.AssemblyQualification);

            return fullyQualifiedName.ToString();
        }
    }
}