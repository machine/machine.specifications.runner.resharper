using Machine.Specifications;

namespace Data
{
    [Behaviors]
    public class TestBehavior
    {
        protected static string item;

        It is_something;
    }

    public class Specs
    {
        protected static string item;

        Behaves_like<TestBehavior> a_class;
    }
}