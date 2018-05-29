using Machine.Specifications;

namespace Data.Reflection
{
    [Subject(typeof(InheritedSubjectsBaseClass))]
    public abstract class InheritedSubjectsBaseClass
    {
    }

    [Subject("specifications")]
    public class InheritedSubjectsSpec : InheritedSubjectsBaseClass
    {
        It is_something;
    }
}
