using Machine.Specifications;

namespace Exploration
{
    [Ignore("this is ignored")]
    class SimpleSpec
    {
        It should_be_true = () =>
            true.ShouldBeTrue();
    }
}
