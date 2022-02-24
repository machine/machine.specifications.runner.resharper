using Machine.Specifications;

namespace Runner.FailingSimple
{
    class SimpleSpec
    {
        It should_be_true = () =>
            true.ShouldBeFalse();
    }
}
