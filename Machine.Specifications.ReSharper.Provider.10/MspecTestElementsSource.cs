using System;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Assemblies.AssemblyToAssemblyResolvers;
using JetBrains.ProjectModel.Assemblies.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider
{
    [SolutionComponent]
    public class MspecTestElementsSource : UnitTestExplorerFrom.DotNetArtefacts, IUnitTestExplorerFromFile
    {
        private readonly SearchDomainFactory _searchDomainFactory;
        private readonly MspecServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public MspecTestElementsSource(
            MspecTestProvider provider,
            SearchDomainFactory searchDomainFactory,
            ISolution solution,
            AssemblyToAssemblyReferencesResolveManager resolveManager,
            ResolveContextManager contextManager,
            MspecServiceProvider serviceProvider,
            ILogger logger)
            : base(solution, provider, resolveManager, contextManager, logger)
        {
            _searchDomainFactory = searchDomainFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override bool IsSupported(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return !project.IsDotNetCoreProject();
        }

        public override void ProcessProject(
            IProject project, 
            FileSystemPath assemblyPath, 
            MetadataLoader loader,
            IUnitTestElementsObserver observer, 
            CancellationToken token)
        {
            var factory = new UnitTestElementFactory(_serviceProvider, project, observer.TargetFrameworkId);
            var explorer = new MspecTestMetadataExplorer(factory, observer);

            MetadataElementsSource.ExploreProject(project, assemblyPath, loader, observer, _logger, token,
                assembly => explorer.ExploreAssembly(assembly, token));

            observer.OnCompleted();
        }

        public void ProcessFile(IFile psiFile, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            var factory = new UnitTestElementFactory(_serviceProvider, psiFile.GetProject(), observer.TargetFrameworkId);
            var explorer = new MspecPsiFileExplorer(_searchDomainFactory, factory, observer, interrupted);

            psiFile.ProcessDescendants(explorer);

            observer.OnCompleted();
        }
    }
}
