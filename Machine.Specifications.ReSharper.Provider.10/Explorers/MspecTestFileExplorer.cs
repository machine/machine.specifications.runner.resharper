namespace Machine.Specifications.ReSharperProvider.Explorers
{
    using System;

    using JetBrains.Application;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Factories;

    public class MspecTestFileExplorer
    {
        readonly ElementFactories _factories;
        readonly MSpecUnitTestProvider _provider;

        public MspecTestFileExplorer(MSpecUnitTestProvider provider,
                                     ElementFactories factories)
        {
            this._provider = provider;
            this._factories = factories;
        }

        public void ExploreFile(IFile psiFile, IUnitTestElementsObserver consumer, Func<bool> interrupted)
        {
            if (psiFile == null)
                throw new ArgumentNullException("psiFile");

            var project = psiFile.GetProject();
            if (project == null)
                return;

            if ((psiFile.Language.Name == "CSHARP") || (psiFile.Language.Name == "VBASIC"))
            {
                psiFile.ProcessDescendants(new FileExplorer(_provider, _factories, psiFile, consumer, interrupted));
            }
        }

        public IUnitTestProvider Provider
        {
            get { return this._provider; }
        }
    }
}