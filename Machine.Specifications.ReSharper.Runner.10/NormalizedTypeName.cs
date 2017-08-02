using System;
using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperRunner
{
    public class NormalizedTypeName
    {
        private static readonly Regex OpenBracketFollowedByDart = new Regex(@"\[.*->\s", RegexOptions.Compiled);
        private static readonly Regex DoubleOpenBrackets = new Regex(@"\[\[", RegexOptions.Compiled);

        private readonly string _normalized;

        public NormalizedTypeName(string typeName)
        {
            _normalized = QualifiedNetNotationWithoutAssembly(typeName);
        }

        public NormalizedTypeName(ITypeOwner field)
        {
            _normalized = QualifiedNetNotationWithoutAssembly(field);
        }

        public override string ToString()
        {
            return _normalized;
        }

        public static implicit operator String(NormalizedTypeName instance)
        {
            return instance.ToString();
        }

        private static string QualifiedNetNotationWithoutAssembly(ITypeOwner field)
        {
            if (field == null)
            {
                throw new ArgumentException("Tried to create normalized type from a null reference.");
            }

            var typeName = field.Type.ToString();
            typeName = typeName.Substring(typeName.IndexOf("-> ") + 3);
            typeName = typeName.Remove(typeName.Length - 1);
            typeName = OpenBracketFollowedByDart.Replace(typeName, "[");

            return typeName;
        }

        private static string QualifiedNetNotationWithoutAssembly(string fullyQualifiedTypeName)
        {
            var typeName = Regex.Replace(fullyQualifiedTypeName, @"\,.+]", "]");
            typeName = DoubleOpenBrackets.Replace(typeName, "[");

            return typeName;
        }
    }
}