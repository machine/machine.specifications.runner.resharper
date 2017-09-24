using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.ReSharperProvider.Reflection;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    public class MspecContext
    {
        private readonly ICollector _collector;

        public MspecContext(ICollector collector)
        {
            _collector = collector;
        }

        public IEnumerable<IFieldInfo> Fields => _collector.Fields;

        public IEnumerable<ITypeInfo> Types => _collector.Types;

        public ITypeInfo Type(string name = null)
        {
            var type = string.IsNullOrEmpty(name)
                ? _collector.Types.FirstOrDefault()
                : _collector.Types.FirstOrDefault(x => x.ShortName == name);

            Assert.That(type, Is.Not.Null, $"Type not found: {name}");

            return type;
        }

        public IFieldInfo Field(string name = null)
        {
            var field = string.IsNullOrEmpty(name)
                ? _collector.Fields.FirstOrDefault()
                : _collector.Fields.FirstOrDefault(x => x.ShortName == name);

            Assert.That(field, Is.Not.Null, $"Field not found: {name}");

            return field;
        }
    }
}