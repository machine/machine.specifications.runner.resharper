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
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestElementsSource : UnitTestExplorerFrom.DotNetArtifacts, IUnitTestExplorerFromFile
    {
        private readonly SearchDomainFactory searchDomainFactory;

        private readonly MspecServiceProvider serviceProvider;

        private readonly ILogger logger;

        public MspecTestElementsSource(
            MspecTestProvider provider,
            SearchDomainFactory searchDomainFactory,
            AssemblyToAssemblyReferencesResolveManager resolveManager,
            ResolveContextManager contextManager,
            MspecServiceProvider serviceProvider,
            ILogger logger)
            : base(provider, resolveManager, contextManager, logger)
        {
            this.searchDomainFactory = searchDomainFactory;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public override PertinenceResult IsSupported(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return PertinenceResult.Yes;
        }

        protected override void ProcessProject(
            IProject project, 
            FileSystemPath assemblyPath, 
            MetadataLoader loader,
            IUnitTestElementsObserver observer, 
            CancellationToken token)
        {
            var factory = new UnitTestElementFactory(serviceProvider, observer.TargetFrameworkId, observer.OnUnitTestElementChanged, UnitTestElementOrigin.Artifact);
            var explorer = new MspecTestMetadataExplorer(factory, observer);

            MetadataElementsSource.ExploreProject(project, assemblyPath, loader, observer, logger, token,
                assembly => explorer.ExploreAssembly(project, assembly, token));
        }

        public void ProcessFile(IFile psiFile, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            var factory = new UnitTestElementFactory(serviceProvider, observer.TargetFrameworkId, observer.OnUnitTestElementChanged, UnitTestElementOrigin.Source);
            var explorer = new MspecPsiFileExplorer(searchDomainFactory, factory, observer, interrupted);

            psiFile.ProcessDescendants(explorer);
        }
    }
}
