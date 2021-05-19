using Machine.Specifications;

namespace Exploration
{
    class SimpleSpec
    {
        [Ignore("this is ignored")]
        It should_be_true = () =>
            true.ShouldBeTrue();
    }
}
