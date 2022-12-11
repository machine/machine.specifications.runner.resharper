using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Elements;

[TestFixture]
public class SpecificationElementTests
{
    [Test]
    public void SetsCorrectIdForSpecification()
    {
        var context = Substitute.For<IContextElement>();
        context.TypeName.Returns("Namespace.ContextType");

        var specification = new SpecificationElement(context, "should_be");

        Assert.That(specification.Id, Is.EqualTo("Namespace.ContextType.should_be"));
    }

    [Test]
    public void SetsCorrectIdForBehaviorSpecification()
    {
        var context = Substitute.For<IContextElement>();
        context.TypeName.Returns("Namespace.ContextType");

        var behavior = Substitute.For<IBehaviorElement>();
        behavior.FieldName.Returns("behaves_like");

        var specification = new SpecificationElement(context, "should_be", null, behavior);

        Assert.That(specification.Id, Is.EqualTo("Namespace.ContextType.behaves_like.should_be"));
    }
}
