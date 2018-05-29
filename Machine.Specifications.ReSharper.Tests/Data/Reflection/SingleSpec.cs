using Machine.Specifications;

namespace Data.Reflection
{
    [Subject(typeof(SingleSpec))]
    class SingleSpec
    {
        static bool value;

        It is_true = () =>
            value.ShouldBeFalse();
    }
}
