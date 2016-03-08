namespace Machine.Specifications.ReSharperProvider.Explorers
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Resources.Shell;
    using JetBrains.ReSharper.UnitTestFramework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class MSpecTestMetadataExplorer
    {
        readonly AssemblyExplorer _assemblyExplorer;
        readonly MSpecUnitTestProvider _provider;

        public MSpecTestMetadataExplorer(MSpecUnitTestProvider provider,
                                         AssemblyExplorer assemblyExplorer)
        {
            this._assemblyExplorer = assemblyExplorer;
            this._provider = provider;
        }

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, IUnitTestElementsObserver consumer, CancellationToken cancellationToken)
        {
            using (ReadLockCookie.Create()) //Get a read lock so that it is safe to read the assembly
            {
                foreach (var metadataTypeInfo in GetTypesIncludingNested(assembly.GetTypes()))
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    this._assemblyExplorer.Explore(project, assembly, consumer, metadataTypeInfo);
                }
            }
        }

        private static IEnumerable<IMetadataTypeInfo> GetTypesIncludingNested(IEnumerable<IMetadataTypeInfo> types)
        {
            foreach (var type in (types ?? Enumerable.Empty<IMetadataTypeInfo>()))
            {
                foreach (var nestedType in GetTypesIncludingNested(type.GetNestedTypes())) //getting nested classes too
                {
                    yield return nestedType;
                }

                yield return type;
            }
        }

        public IUnitTestProvider Provider
        {
            get { return this._provider; }
        }
    }
}