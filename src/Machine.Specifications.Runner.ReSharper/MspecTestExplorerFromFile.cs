using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestExplorerFromFile : IUnitTestExplorerFromFile
    {
        private readonly SearchDomainFactory searchDomainFactory;

        private readonly MspecServiceProvider serviceProvider;

        public MspecTestExplorerFromFile(MspecServiceProvider serviceProvider, SearchDomainFactory searchDomainFactory)
        {
            this.searchDomainFactory = searchDomainFactory;
            this.serviceProvider = serviceProvider;
        }

        public IUnitTestProvider Provider => serviceProvider.Provider;

        public void ProcessFile(IFile psiFile, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            if (!IsProjectFile(psiFile))
            {
                return;
            }

            var factory = new UnitTestElementFactory(serviceProvider, observer.TargetFrameworkId, observer.OnUnitTestElementChanged, UnitTestElementOrigin.Source);
            var explorer = new MspecPsiFileExplorer(searchDomainFactory, factory, observer, interrupted);

            psiFile.ProcessDescendants(explorer);
        }

        private static bool IsProjectFile(IFile psiFile)
        {
            return psiFile.GetSourceFile().ToProjectFile() != null;
        }
    }
}
