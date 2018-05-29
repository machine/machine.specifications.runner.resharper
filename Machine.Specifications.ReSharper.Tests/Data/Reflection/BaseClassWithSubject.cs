﻿using Machine.Specifications;

namespace Data.Reflection
{
    [Subject(typeof(BaseClassWithSubjectBaseClass))]
    class BaseClassWithSubjectBaseClass
    {
        protected static bool value;
    }

    class BaseClassWithSubjectSpec : BaseClassWithSubjectBaseClass
    {
        It is_true = () =>
            value.ShouldBeFalse();
    }
}
