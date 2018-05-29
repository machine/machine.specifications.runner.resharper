using Machine.Specifications;

namespace Data.Psi
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
