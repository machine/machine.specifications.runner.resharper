using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework;

[TestFixture]
[Category("Unit Test support")]
public abstract class UnitTestMetadataTestBase : UnitTestElementDiscoveryTestBase
{
    protected override string GetIdString(IUnitTestElement element)
    {
        return $"{element.NaturalId.ProviderId}::{element.NaturalId.ProjectId}::{element.NaturalId.TestId}";
    }

    protected override void DoTest(Lifetime lifetime, IProject testProject)
    {
        var name = testProject.GetSubItems()[0].Location.Name;

        var testAssembly = GetVirtualTestDataFilePath(name);

        var resolve = new AssemblyResolverOnFolders();
        resolve.AddPath(testAssembly.Directory);

        var discoveryManager = Solution.GetComponent<IUnitTestDiscoveryManager>();
        var assemblyExplorer = Solution.GetComponent<MspecTestExplorerFromArtifacts>();

        var source = new UnitTestElementSource(UnitTestElementOrigin.Artifact,
            new ExplorationTarget(
                testProject,
                GetTargetFrameworkId(),
                new MspecTestProvider()));

        using var transaction = discoveryManager.BeginTransaction(source);

        assemblyExplorer.ProcessArtifact(transaction.Observer, lifetime).Wait(lifetime);

        DumpElements(transaction.Elements, name + ".metadata");
    }
}
