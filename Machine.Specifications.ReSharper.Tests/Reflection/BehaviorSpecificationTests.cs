using System.Linq;
using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Reflection
{
    [TestFixture]
    public class BehaviorSpecificationTests : ReflectionWithSingleProject
    {
        [Test]
        public void ClassHasBehavior()
        {
            WithFile("SingleBehaviorSpec.cs", x => Assert.That(x.Field("a_class").IsBehavior(), Is.True));
        }

        [Test]
        public void CanGetBehaviorType()
        {
            WithFile("SingleBehaviorSpec.cs", x => Assert.That(x.Field("a_class").FieldType, Is.Not.Null));
        }

        [Test]
        public void ClassHasBehaviorSpecs()
        {
            WithFile("SingleBehaviorSpec.cs", x =>
            {
                var fieldType = x.Field("a_class").FieldType;

                var spec = fieldType.GetGenericArguments()
                    .FirstOrDefault()?
                    .GetFields()
                    .FirstOrDefault(y => y.ShortName == "is_something");

                Assert.That(spec, Is.Not.Null);
                Assert.That(spec.IsSpecification(), Is.True);
            });
        }
    }
}
