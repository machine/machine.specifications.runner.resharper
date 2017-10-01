using System.Collections.Generic;
using JetBrains.Metadata.Reader.Impl;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Explorers
{
    [TestFixture]
    public class ClrTypeNameTests
    {
        [Test]
        public void DifferentTypeNamesAreUnique()
        {
            var name1 = new ClrTypeName("Namespace.Name1");
            var name2 = new ClrTypeName("Namespace.Name2");

            var dictionary = new Dictionary<ClrTypeName, int>();
            dictionary[name1] = 1;
            dictionary[name2] = 2;

            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(name1, Is.Not.EqualTo(name2));
        }

        [Test]
        public void SameTypeNamesCanBeAKey()
        {
            var name1 = new ClrTypeName("Namespace.Name1");
            var name2 = new ClrTypeName("Namespace.Name1");

            var dictionary = new Dictionary<ClrTypeName, int>();
            dictionary[name1] = 1;
            dictionary[name2] = 2;

            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(name1, Is.EqualTo(name2));
        }
    }
}