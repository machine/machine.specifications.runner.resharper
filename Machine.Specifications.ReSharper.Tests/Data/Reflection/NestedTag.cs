using Machine.Specifications;

namespace Data.Reflection
{
    [Tags("OuterTags")]
    public class NestedTagSpecs
    {
        [Tags("InnerTags")]
        class InnerSpecs
        {
            It is_something;
        }
    }
}
