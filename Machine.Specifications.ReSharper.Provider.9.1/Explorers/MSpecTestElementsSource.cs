using JetBrains.ReSharper.Psi;
using Machine.Specifications.ReSharperProvider.Factories;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Application;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;
    using JetBrains.Util.Logging;

    [SolutionComponent]
    public class MSpecTestElementsSource : IUnitTestElementsSource
    {
        private readonly MSpecUnitTestProvider _provider;
        private readonly AssemblyExplorer _assemblyExplorer;
        private readonly ElementFactories _elementFactories;
        private readonly MetadataElementsSource _metadataElementsSource;

        public MSpecTestElementsSource(MSpecUnitTestProvider provider, AssemblyExplorer assemblyExplorer, ElementFactories elementFactories, IShellLocks shellLocks)
        {
            this._provider = provider;
            this._assemblyExplorer = assemblyExplorer;
            this._elementFactories = elementFactories;

            _metadataElementsSource = new MetadataElementsSource(Logger.GetLogger(typeof(MSpecTestElementsSource)),
                shellLocks);
        }

        public void ExploreSolution(IUnitTestElementsObserver observer)
        {
        }

        public void ExploreProjects(IDictionary<IProject, FileSystemPath> projects, MetadataLoader loader, IUnitTestElementsObserver observer)
        {
            var explorer = new MSpecTestMetadataExplorer(_provider, _assemblyExplorer);
            _metadataElementsSource.ExploreProjects(projects, loader, observer, explorer.ExploreAssembly);
            observer.OnCompleted();
        }

        public void ExploreFile(IFile psiFile, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            if (!IsProjectFile(psiFile)) return;

            var explorer = new MspecTestFileExplorer(_provider, _elementFactories);
            explorer.ExploreFile(psiFile, observer, interrupted);
            observer.OnCompleted();
        }

        public IUnitTestProvider Provider
        {
            get { return _provider; }
        }

        private static bool IsProjectFile(IFile psiFile)
        {
            // Can return null for external sources
            var projectFile = psiFile.GetSourceFile().ToProjectFile();
            return projectFile != null;
        }
    }
}