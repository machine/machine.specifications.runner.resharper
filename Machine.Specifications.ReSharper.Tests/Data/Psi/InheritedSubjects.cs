using Machine.Specifications;

namespace Data.Psi
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
