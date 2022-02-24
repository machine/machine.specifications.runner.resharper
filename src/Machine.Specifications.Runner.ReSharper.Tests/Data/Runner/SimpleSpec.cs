using Machine.Specifications;

namespace Runner.PassingSimple
{
    class SimpleSpec
    {
        It should_be_true = () =>
            true.ShouldBeTrue();
    }
}
