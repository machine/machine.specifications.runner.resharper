using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.Runner.ReSharper.Elements;

namespace Machine.Specifications.Runner.ReSharper
{
    public static class UnitTestElementManagerExtensions
    {
        public static T GetElementById<T>(this IUnitTestElementManager manager, UnitTestElementId id)
            where T : MspecTestElement
        {
            return manager.GetElementById(id) as T;
        }
    }
}
