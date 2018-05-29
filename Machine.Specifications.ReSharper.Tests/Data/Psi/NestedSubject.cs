using Machine.Specifications;

namespace Data.Psi
{
    [Subject(typeof(NestedSubjectBaseClass))]
    public abstract class NestedSubjectBaseClass
    {
    }

    [Subject(typeof(NestedSubjectSpecs))]
    public class NestedSubjectSpecs : NestedSubjectBaseClass
    {

        [Subject(typeof(InnerSpecs), "plus")]
        class InnerSpecs
        {
            It is_something;
        }
    }
}
