using Machine.Specifications;

namespace Data.Psi
{
    public class IgnoredFieldSpecs
    {
        [Ignore("reason")]
        It is_something;
    }
}
