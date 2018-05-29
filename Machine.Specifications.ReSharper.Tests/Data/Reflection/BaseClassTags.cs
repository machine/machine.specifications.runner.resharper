using Machine.Specifications;

namespace Data.Reflection
{
    [Tags("Taggy")]
    class BaseClassTagsBaseClass
    {
        protected static bool value;
    }

    class BaseClassTagsSpec : BaseClassTagsBaseClass
    {
        It is_true = () =>
            value.ShouldBeFalse();
    }
}
