using System;
using System.Text.RegularExpressions;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperRunner
{
    public class NormalizedTypeName
    {
        readonly string _normalized;
        static readonly Regex OpenBracketFollowedByDart = new Regex(@"\[.*->\s", RegexOptions.Compiled);
        static readonly Regex DoubleOpenBrackets = new Regex(@"\[\[", RegexOptions.Compiled);

        public NormalizedTypeName(string typeName)
        {
            this._normalized = QualifiedNetNotationWithoutAssembly(typeName);
        }

        public NormalizedTypeName(IClrTypeName clrTypeName)
        {
            this._normalized = QualifiedNetNotationWithoutAssembly(clrTypeName.FullName);
        }

        public NormalizedTypeName(ITypeOwner field)
        {
            this._normalized = QualifiedNetNotationWithoutAssembly(field);
        }

        public override string ToString()
        {
            return this._normalized;
        }

        public static implicit operator String(NormalizedTypeName instance)
        {
            return instance.ToString();
        }

        static string QualifiedNetNotationWithoutAssembly(ITypeOwner field)
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

        static string QualifiedNetNotationWithoutAssembly(string fullyQualifiedTypeName)
        {
            var typeName = Regex.Replace(fullyQualifiedTypeName, @"\,.+]", "]");
            typeName = DoubleOpenBrackets.Replace(typeName, "[");
            return typeName;
        }
    }
}