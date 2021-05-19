using Machine.Specifications;

namespace Exploration
{
    class SimpleSpec
    {
        It should_be_true = () =>
            true.ShouldBeTrue();
    }
}
