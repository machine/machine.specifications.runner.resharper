using System.Linq;
using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Reflection
{
    [TestFixture]
    public class TagTests : PsiTests
    {
        [Test]
        public void TagsInBaseClassIgnored()
        {
            WithFile("BaseClassTags.cs", x => Assert.That(Type("Spec").GetTags(), Is.Empty));
        }

        [Test]
        public void TagsInContainingClassIgnored()
        {
            WithFile("ContainingClassTags.cs", x => Assert.That(Type("InnerSpecs").GetTags(), Is.Empty));
        }

        [Test]
        public void TagsFromInnerClassUsed()
        {
            WithFile("NestedTag.cs", x =>
            {
                var tags = Type("InnerSpecs").GetTags().ToArray();

                Assert.That(tags, Is.Not.Empty);
                Assert.That(tags.Length, Is.EqualTo(1));
                Assert.That(tags, Contains.Item("InnerTags"));
            });
        }

        [Test]
        public void SingleTagNameMatched()
        {
            WithFile("SingleTag.cs", x =>
            {
                var tags = Type().GetTags().ToArray();

                Assert.That(tags.Length, Is.EqualTo(1));
                Assert.That(tags, Contains.Item("Taggy"));
            });
        }

        [Test]
        public void MultipleTagsMatched()
        {
            WithFile("MultipleTags.cs", x =>
            {
                var tags = Type().GetTags().ToArray();

                Assert.That(tags.Length, Is.EqualTo(3));
                Assert.That(tags, Contains.Item("Taggy1"));
                Assert.That(tags, Contains.Item("Taggy2"));
                Assert.That(tags, Contains.Item("Taggy3"));
            });
        }

        [Test]
        public void DuplicateTagsIgnored()
        {
            WithFile("DuplicateTags.cs", x =>
            {
                var tags = Type().GetTags().ToArray();

                Assert.That(tags.Length, Is.EqualTo(2));
                Assert.That(tags, Contains.Item("Taggy"));
                Assert.That(tags, Contains.Item("Taggy2"));
            });
        }
    }
}
