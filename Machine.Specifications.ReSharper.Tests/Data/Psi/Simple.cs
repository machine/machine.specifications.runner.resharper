using Machine.Specifications;

namespace Data
{
    [Subject(typeof(Simple))]
    class Simple
    {
        static bool value;

        It is_true = () =>
            value.ShouldBeFalse();
    }
}