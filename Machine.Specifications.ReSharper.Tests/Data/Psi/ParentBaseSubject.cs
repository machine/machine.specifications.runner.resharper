using Machine.Specifications;

namespace Data.Psi
{
    [Subject(typeof(ParentBaseSubjectBaseClass))]
    public abstract class ParentBaseSubjectBaseClass
    {
    }

    public class ParentBaseSubjectSpecs : ParentBaseSubjectBaseClass
    {
        class InnerSpecs
        {
            It is_something;
        }
    }
}
