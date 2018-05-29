using Machine.Specifications;

namespace Data.Psi
{
    [Subject(typeof(SingleSpec))]
    class SingleSpec
    {
        static bool value;

        It is_true = () =>
            value.ShouldBeFalse();
    }
}
