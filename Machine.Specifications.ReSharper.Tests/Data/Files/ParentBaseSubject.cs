using Machine.Specifications;

namespace Data
{
    [Subject(typeof(BaseClass))]
    public abstract class BaseClass
    {
    }

    public class Specs : BaseClass
    {
        class InnerSpecs
        {
            It is_something;
        }
    }
}
