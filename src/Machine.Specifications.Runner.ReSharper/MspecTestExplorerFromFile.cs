using System;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Daemon;

namespace Machine.Specifications.Runner.ReSharper;

[SolutionComponent(Instantiation.DemandAnyThreadUnsafe)]
public class MspecTestExplorerFromFile(MspecTestProvider provider) : IUnitTestExplorerFromFile
{
    public IUnitTestProvider Provider { get; } = provider;

    public void ProcessFile(IFile psiFile, IUnitTestElementObserverOnFile observer, Func<bool> interrupted)
    {
        if (!IsProjectFile(psiFile))
        {
            return;
        }

        var explorer = new MspecPsiFileExplorer(observer, interrupted);

        psiFile.ProcessDescendants(explorer);
    }

    private static bool IsProjectFile(IFile psiFile)
    {
        return psiFile.GetSourceFile().ToProjectFile() != null;
    }
}
