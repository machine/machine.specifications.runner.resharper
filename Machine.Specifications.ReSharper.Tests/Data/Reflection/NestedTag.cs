using Machine.Specifications;

namespace Data
{
    [Tags("OuterTags")]
    public class Specs
    {
        [Tags("InnerTags")]
        class InnerSpecs
        {
            It is_something;
        }
    }
}
