using System.Collections.Generic;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Assemblies.AssemblyToAssemblyResolvers;
using JetBrains.ProjectModel.Assemblies.Impl;
using JetBrains.ProjectModel.NuGet.Packaging;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestExplorerFromMetadata : UnitTestExplorerFrom.Metadata
    {
        private readonly MspecServiceProvider serviceProvider;

        private readonly ILogger logger;

        public MspecTestExplorerFromMetadata(
            MspecServiceProvider serviceProvider,
            AssemblyToAssemblyReferencesResolveManager resolveManager,
            ResolveContextManager resolveContextManager,
            NuGetInstalledPackageChecker installedPackageChecker,
            ILogger logger)
            : base(serviceProvider.Provider, resolveManager, resolveContextManager, installedPackageChecker, logger)
        {
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
    }
}
