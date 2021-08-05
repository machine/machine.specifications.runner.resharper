using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.Runner.ReSharper.Elements;

namespace Machine.Specifications.Runner.ReSharper
{
    public static class UnitTestElementExtensions
    {
        public static string? GetExplicitReason(this IUnitTestElement element)
        {
            if (element is MspecTestElement mspecElement)
            {
                return mspecElement.ExplicitReason;
            }

            return null;
        }
    }
}
