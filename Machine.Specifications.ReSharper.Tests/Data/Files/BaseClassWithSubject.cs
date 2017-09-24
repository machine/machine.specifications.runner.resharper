using Machine.Specifications;

namespace Data
{
    [Subject(typeof(BaseClass))]
    class BaseClass
    {
        static bool value;
    }

    class Spec : BaseClass
    {
        It is_true = () =>
            value.ShouldBeFalse();
    }
}
