using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework;

public class TestElementObserverOnFile(IUnitTestElementObserver inner, IPsiSourceFile sourceFile)
    : IUnitTestElementObserverOnFile
{
    public IUnitTestElementSource Source => inner.Source;

    public IPsiSourceFile PsiSourceFile { get; } = sourceFile;

    public T GetElementById<T>(string testId, string? salt = null)
    {
        return inner.GetElementById<T>(testId,  salt);
    }

    public void OnUnitTestElement(IUnitTestElement element)
    {
        inner.OnUnitTestElement(element);
    }

    public void OnUnitTestElementDisposition(IUnitTestLikeElement element, TextRange navigationRange, TextRange containingRange)
    {
    }
}
