using Machine.Specifications;

namespace Data.Reflection
{
    public class IgnoredFieldSpecs
    {
        [Ignore("reason")]
        It is_something;
    }
}
