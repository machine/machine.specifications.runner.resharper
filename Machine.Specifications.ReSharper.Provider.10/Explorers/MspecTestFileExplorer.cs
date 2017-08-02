using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Factories;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    public class MspecTestFileExplorer
    {
        private readonly ElementFactories _factories;
        private readonly MSpecUnitTestProvider _provider;

        public MspecTestFileExplorer(MSpecUnitTestProvider provider, ElementFactories factories)
        {
            _provider = provider;
            _factories = factories;
        }

        public void ExploreFile(IFile psiFile, IUnitTestElementsObserver consumer, Func<bool> interrupted)
        {
            if (psiFile == null)
                throw new ArgumentNullException(nameof(psiFile));

            var project = psiFile.GetProject();

            if (project == null)
                return;

            if (psiFile.Language.Name == "CSHARP" || psiFile.Language.Name == "VBASIC")
                psiFile.ProcessDescendants(new FileExplorer(_provider, _factories, psiFile, consumer, interrupted));
        }
    }
}