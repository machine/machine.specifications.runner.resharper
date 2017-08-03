using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    public class MSpecTestMetadataExplorer
    {
        private readonly AssemblyExplorer _assemblyExplorer;

        public MSpecTestMetadataExplorer(AssemblyExplorer assemblyExplorer)
        {
            _assemblyExplorer = assemblyExplorer;
        }

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, IUnitTestElementsObserver consumer, CancellationToken cancellationToken)
        {
            //Get a read lock so that it is safe to read the assembly
            using (ReadLockCookie.Create())
            {
                foreach (var metadataTypeInfo in GetTypesIncludingNested(assembly.GetTypes()))
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    _assemblyExplorer.Explore(project, assembly, consumer, metadataTypeInfo);
                }
            }
        }

        private static IEnumerable<IMetadataTypeInfo> GetTypesIncludingNested(IEnumerable<IMetadataTypeInfo> types)
        {
            foreach (var type in types ?? Enumerable.Empty<IMetadataTypeInfo>())
            {
                //getting nested classes too
                foreach (var nestedType in GetTypesIncludingNested(type.GetNestedTypes()))
                    yield return nestedType;

                yield return type;
            }
        }
    }
}