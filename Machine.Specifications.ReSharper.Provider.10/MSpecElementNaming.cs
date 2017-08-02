using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming.Elements;

namespace Machine.Specifications.ReSharperProvider
{
    [NamedElementsBag]
    public class MSpecElementNaming : ElementKindOfElementType
    {
        protected MSpecElementNaming(string name, string presentableName, Func<IDeclaredElement, bool> isApplicable)
            : base(name, presentableName, isApplicable)
        {
        }
    }
}