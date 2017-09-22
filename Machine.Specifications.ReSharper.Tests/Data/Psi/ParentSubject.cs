using Machine.Specifications;

namespace Data
{
    [Subject(typeof(BaseClass))]
    public abstract class BaseClass
    {
    }

    [Subject(typeof(Specs))]
    public class Specs : BaseClass
    {
        class InnerSpecs
        {
            It is_something;
        }
    }
}