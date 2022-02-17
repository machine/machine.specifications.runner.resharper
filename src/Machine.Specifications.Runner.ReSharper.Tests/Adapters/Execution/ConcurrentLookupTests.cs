﻿using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Execution
{
    public class ConcurrentLookupTests
    {
        [Test]
        public void CanAddWithSameAggregateId()
        {
            var lookup = new ConcurrentLookup<ContextElement>();

            lookup.Add(new ContextElement("Type", "Subject"));
            lookup.Add(new ContextElement("Type", "SubjectS"));
        }

        [Test]
        public void CanTakeWhenUsingDuplicateIds()
        {
            var lookup = new ConcurrentLookup<ContextElement>();

            lookup.Add(new ContextElement("Type", "Subject"));
            lookup.Add(new ContextElement("Type", "SubjectS"));

            Assert.NotNull(lookup.Take("Type"));
            Assert.NotNull(lookup.Take("Type"));
        }

        [Test]
        public void ReturnsNullWhenAllTaken()
        {
            var lookup = new ConcurrentLookup<ContextElement>();

            lookup.Add(new ContextElement("Type", "Subject"));
            lookup.Add(new ContextElement("Type", "SubjectS"));

            Assert.NotNull(lookup.Take("Type"));
            Assert.NotNull(lookup.Take("Type"));
            Assert.Null(lookup.Take("Type"));
        }
    }
}
