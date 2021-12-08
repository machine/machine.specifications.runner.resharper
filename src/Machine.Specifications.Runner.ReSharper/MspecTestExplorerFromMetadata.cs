using System.Collections.Generic;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Assemblies.AssemblyToAssemblyResolvers;
using JetBrains.ProjectModel.Assemblies.Impl;
using JetBrains.ProjectModel.NuGet.Packaging;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestExplorerFromMetadata : UnitTestExplorerFrom.Metadata
    {
        private readonly ILogger logger;

        public MspecTestExplorerFromMetadata(
            MspecTestProvider provider,
            AssemblyToAssemblyReferencesResolveManager resolveManager,
            ResolveContextManager resolveContextManager,
            NuGetInstalledPackageChecker installedPackageChecker,
            ILogger logger)
            : base(provider, resolveManager, resolveContextManager, installedPackageChecker, logger)
        {
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
            MetadataLoader loader,
            IUnitTestElementObserver observer, 
            CancellationToken token)
        {
            MetadataElementsSource.ExploreProject(observer.Source.Project, observer.Source.Output, loader, logger, token, assembly =>
            {
                var explorer = new MspecTestMetadataExplorer(observer);

                explorer.ExploreAssembly(assembly, token);
            });
        }
    }
}
