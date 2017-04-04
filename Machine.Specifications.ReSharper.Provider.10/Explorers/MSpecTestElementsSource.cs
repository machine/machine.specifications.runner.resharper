using System.Threading;
using JetBrains.ProjectModel.Assemblies.AssemblyToAssemblyResolvers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
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
    public class MSpecTestElementsSource : UnitTestExplorerFrom.DotNetArtefacts, IUnitTestExplorerFromFile
    {
        private readonly MSpecUnitTestProvider _provider;
        private readonly AssemblyExplorer _assemblyExplorer;
        private readonly ElementFactories _elementFactories;
        private readonly ILogger _logger;

        public MSpecTestElementsSource(MSpecUnitTestProvider provider, AssemblyExplorer assemblyExplorer, ElementFactories elementFactories, ISolution solution, AssemblyToAssemblyReferencesResolveManager resolveManager, ILogger logger)
            : base(solution, provider, resolveManager, logger)
        {
            this._provider = provider;
            this._assemblyExplorer = assemblyExplorer;
            this._elementFactories = elementFactories;
            this._logger = logger;
        }
        
        public override void ProcessProject(IProject project, FileSystemPath assemblyPath, MetadataLoader loader, IUnitTestElementsObserver observer, CancellationToken token)
        {
            var explorer = new MSpecTestMetadataExplorer(_provider, _assemblyExplorer);

            MetadataElementsSource.ExploreProject(project, assemblyPath, loader, observer, _logger, token,
                assembly => explorer.ExploreAssembly(project, assembly, observer, token));
            observer.OnCompleted();
        }

        public void ProcessFile(IFile psiFile, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            if (!IsProjectFile(psiFile)) return;

            var explorer = new MspecTestFileExplorer(_provider, _elementFactories);
            explorer.ExploreFile(psiFile, observer, interrupted);
            observer.OnCompleted();
        }
        
        private static bool IsProjectFile(IFile psiFile)
        {
            // Can return null for external sources
            var projectFile = psiFile.GetSourceFile().ToProjectFile();
            return projectFile != null;
        }
    }
}