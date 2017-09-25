using Machine.Specifications;

namespace Data
{
    [Subject(typeof(BaseClass))]
    class BaseClass
    {
        protected static bool value;
    }

    class Spec : BaseClass
    {
        It is_true = () =>
            value.ShouldBeFalse();
    }
}
