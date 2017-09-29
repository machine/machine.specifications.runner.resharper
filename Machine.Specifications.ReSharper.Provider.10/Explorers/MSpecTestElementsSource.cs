using System;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Assemblies.AssemblyToAssemblyResolvers;
using JetBrains.ProjectModel.Assemblies.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider.Factories;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    [SolutionComponent]
    public class MSpecTestElementsSource : UnitTestExplorerFrom.DotNetArtefacts, IUnitTestExplorerFromFile
    {
        private readonly MspecTestProvider _provider;
        private readonly AssemblyExplorer _assemblyExplorer;
        private readonly ElementFactories _elementFactories;
        private readonly ILogger _logger;

        public MSpecTestElementsSource(MspecTestProvider provider, AssemblyExplorer assemblyExplorer, ElementFactories elementFactories, ISolution solution, AssemblyToAssemblyReferencesResolveManager resolveManager, ResolveContextManager contextManager, ILogger logger)
            : base(solution, provider, resolveManager, contextManager, logger)
        {
            _provider = provider;
            _assemblyExplorer = assemblyExplorer;
            _elementFactories = elementFactories;
            _logger = logger;
        }
        
        public override void ProcessProject(IProject project, FileSystemPath assemblyPath, MetadataLoader loader, IUnitTestElementsObserver observer, CancellationToken token)
        {
            var explorer = new MSpecTestMetadataExplorer(_assemblyExplorer);

            MetadataElementsSource.ExploreProject(project, assemblyPath, loader, observer, _logger, token,
                assembly => explorer.ExploreAssembly(project, assembly, observer, token));

            observer.OnCompleted();
        }

        public void ProcessFile(IFile psiFile, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            if (!IsProjectFile(psiFile))
                return;

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