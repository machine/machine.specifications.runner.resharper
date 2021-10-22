using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Daemon;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestExplorerFromFile : IUnitTestExplorerFromFile
    {
        private readonly SearchDomainFactory searchDomainFactory;

        private readonly MspecServiceProvider services;

        public MspecTestExplorerFromFile(MspecServiceProvider services, SearchDomainFactory searchDomainFactory)
        {
            this.searchDomainFactory = searchDomainFactory;
            this.services = services;
        }

        public IUnitTestProvider Provider => services.Provider;

        public void ProcessFile(IFile psiFile, IUnitTestElementObserverOnFile observer, Func<bool> interrupted)
        {
            if (!IsProjectFile(psiFile))
            {
                return;
            }

            var explorer = new MspecPsiFileExplorer(searchDomainFactory, observer, interrupted);

            psiFile.ProcessDescendants(explorer);
        }

        private static bool IsProjectFile(IFile psiFile)
        {
            return psiFile.GetSourceFile().ToProjectFile() != null;
        }
    }
}
