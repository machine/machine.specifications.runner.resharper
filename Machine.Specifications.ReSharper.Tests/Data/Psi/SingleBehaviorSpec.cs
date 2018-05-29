using Machine.Specifications;

namespace Data.Psi
{
    [Behaviors]
    public class TestBehavior
    {
        protected static string item;

        It is_something;
    }

    public class SingleBehaviorSpec
    {
        protected static string item;

        Behaves_like<TestBehavior> a_class;
    }
}
