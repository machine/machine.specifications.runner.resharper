using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.ReSharperProvider.Reflection;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    public class MspecContext
    {
        public MspecContext(ITypeInfo[] types, IFieldInfo[] fields)
        {
            Types = types;
            Fields = fields;
        }

        public IEnumerable<IFieldInfo> Fields { get; }

        public IEnumerable<ITypeInfo> Types { get; }

        public ITypeInfo Type(string name = null)
        {
            var type = string.IsNullOrEmpty(name)
                ? Types.FirstOrDefault()
                : Types.FirstOrDefault(x => x.ShortName == name);

            Assert.That(type, Is.Not.Null, $"Type not found: {name}");

            return type;
        }

        public IFieldInfo Field(string name = null)
        {
            var field = string.IsNullOrEmpty(name)
                ? Fields.FirstOrDefault()
                : Fields.FirstOrDefault(x => x.ShortName == name);

            Assert.That(field, Is.Not.Null, $"Field not found: {name}");

            return field;
        }
    }
}
