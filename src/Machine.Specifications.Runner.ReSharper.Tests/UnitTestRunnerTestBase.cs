using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.TestFramework;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.ReSharper.UnitTestFramework.Execution.Hosting;
using JetBrains.ReSharper.UnitTestFramework.Execution.Launch;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [EnsureUnitTestRepositoryIsEmpty]
    public abstract class UnitTestRunnerTestBase : BaseTestWithSingleProject
    {
        public override void SetUp()
        {
            var factoryMethod = typeof(Logger).GetProperty(nameof(Logger.Factory), BindingFlags.Static | BindingFlags.Public);

            if (factoryMethod != null)
            {
                var factory = factoryMethod.GetValue(null);

                if (factory == null)
                {
                    factoryMethod.SetValue(null, new LoggerFactory());
                }
            }
        }

        protected override void DoTest(Lifetime lifetime, IProject testProject)
        {
            var facade = Solution.GetComponent<IUnitTestingFacade>();

            var projectFile = testProject.GetSubItems().First();

            CopyFrameworkLibrary(projectFile);

            ExecuteWithGold(projectFile.Location.FullPath, output =>
            {
                var elements = GetUnitTestElements(testProject).ToArray();

                var session = facade.SessionRepository.CreateSession(SolutionCriterion.Instance);
                facade.SessionManager.OpenSession(session);

                var launch = facade.LaunchManager.CreateLaunch(
                    session,
                    new UnitTestElements(SolutionCriterion.Instance, elements),
                    UnitTestHost.Instance.GetProvider("Process"));

                launch.Run().Wait(lifetime);

                WriteResults(elements, output);
            });
        }

        private ICollection<IUnitTestElement> GetUnitTestElements(IProject testProject)
        {
            var provider = UT.Facade.ProviderCache.GetProviderByProviderId(MspecTestProvider.Id);

            var source = new UnitTestElementSource(UnitTestElementOrigin.Source,
                new ExplorationTarget(
                    testProject,
                    GetTargetFrameworkId(),
                    provider));

            var discoveryManager = Solution.GetComponent<IUnitTestDiscoveryManager>();
            var metadataExplorer = Solution.GetComponent<MspecTestExplorerFromArtifacts>();

            using (var transaction = discoveryManager.BeginTransaction(source))
            {
                metadataExplorer.ProcessArtifact(transaction.Observer, CancellationToken.None).Wait();

                transaction.Commit();

                return transaction.Elements;
            }
        }

        private void WriteResults(IUnitTestElement[] elements, TextWriter output)
        {
            var results = Solution.GetComponent<IUnitTestResultManager>();

            foreach (var element in elements)
            {
                var result = results.GetResult(element);

                output.WriteLine($"{element.NaturalId.ProviderId}::{element.NaturalId.ProjectId}::{element.NaturalId.TestId}");
                output.Write("  ");
                output.WriteLine(result.ToString());
            }
        }

        private void CopyFrameworkLibrary(IProjectItem project)
        {
            var assemblies = GetReferencedAssemblies(GetTargetFrameworkId())
                .Where(Path.IsPathRooted);

            foreach (var assembly in assemblies)
            {
                var source = FileSystemPath.Parse(assembly);
                var target = project.Location.Directory.Combine(source.Name);

                if (source.ExistsFile)
                {
                    source.CopyFile(target.ToNativeFileSystemPath(), true);
                }
            }
        }
    }
}
