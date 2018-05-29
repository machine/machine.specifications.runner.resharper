using Machine.Specifications;

namespace Data.Reflection
{
    [Subject(typeof(ParentSubjectBaseClass))]
    public abstract class ParentSubjectBaseClass
    {
    }

    [Subject(typeof(ParentSubjectSpecs))]
    public class ParentSubjectSpecs : ParentSubjectBaseClass
    {
        class InnerSpecs
        {
            It is_something;
        }
    }
}
