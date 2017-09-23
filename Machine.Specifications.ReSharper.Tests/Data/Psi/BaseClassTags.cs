using Machine.Specifications;

namespace Data
{
    [Tags("Taggy")]
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
