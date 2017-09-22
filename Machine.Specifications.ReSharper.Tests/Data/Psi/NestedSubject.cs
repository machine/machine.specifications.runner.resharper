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

        [Subject(typeof(InnerSpecs), "plus")]
        class InnerSpecs
        {
            It is_something;
        }
    }
}