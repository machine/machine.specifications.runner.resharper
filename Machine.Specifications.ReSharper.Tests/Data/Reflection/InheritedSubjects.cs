using Machine.Specifications;

namespace Data
{
    [Subject(typeof(BaseClass))]
    public abstract class BaseClass
    {
    }

    [Subject("specifications")]
    public class Spec : BaseClass
    {
        It is_something;
    }
}
