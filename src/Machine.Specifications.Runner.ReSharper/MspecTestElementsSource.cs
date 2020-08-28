using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Assemblies.AssemblyToAssemblyResolvers;
using JetBrains.ProjectModel.Assemblies.Impl;
using JetBrains.ProjectModel.NuGet.Packaging;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestElementsSource : TestRunnerArtifactExplorer, IUnitTestExplorerFromFile
    {
        private readonly SearchDomainFactory searchDomainFactory;

        private readonly MspecServiceProvider serviceProvider;

        private readonly ILogger logger;

        public MspecTestElementsSource(
            MspecTestProvider provider,
            MspecServiceProvider serviceProvider,
            SearchDomainFactory searchDomainFactory,
            AssemblyToAssemblyReferencesResolveManager resolveManager,
            NuGetInstalledPackageChecker installedPackageChecker,
            ResolveContextManager contextManager,
            ILogger logger)
            : base(provider, resolveManager, contextManager, installedPackageChecker, logger)
        {
            this.searchDomainFactory = searchDomainFactory;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        protected override IEnumerable<string> GetRequiredNuGetDependencies(IProject project, TargetFrameworkId targetFrameworkId)
        {
            foreach (var dependency in base.GetRequiredNuGetDependencies(project, targetFrameworkId))
            {
                yield return dependency;
            }

            yield return "Machine.Specifications.Runner.VisualStudio";
        }

        protected override void ProcessProject(
            IProject project, 
            FileSystemPath assemblyPath, 
            MetadataLoader loader,
            IUnitTestElementsObserver observer, 
            CancellationToken token)
        {
            var factory = new UnitTestElementFactory(serviceProvider, observer.TargetFrameworkId, observer.OnUnitTestElementChanged, UnitTestElementOrigin.Artifact);
            var explorer = new MspecTestMetadataExplorer(project, factory, observer);

            MetadataElementsSource.ExploreProject(project, assemblyPath, loader, observer, logger, token,
                assembly => explorer.ExploreAssembly(assembly, token));
        }

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
