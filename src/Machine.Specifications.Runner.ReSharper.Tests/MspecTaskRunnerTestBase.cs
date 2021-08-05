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
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [EnsureUnitTestRepositoryIsEmpty]
    public abstract class MspecTaskRunnerTestBase : BaseTestWithSingleProject
    {
        public IUnitTestExplorerFromArtifacts MetadataExplorer => Solution.GetComponent<MspecTestExplorerFromArtifacts>();

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
            var elementManager = Solution.GetComponent<IUnitTestElementManager>();

            var projectFile = testProject.GetSubItems().First();

            CopyFrameworkLibrary(projectFile);

            ExecuteWithGold(projectFile.Location.FullPath, output =>
            {
                var elements = GetUnitTestElements(testProject, projectFile.Location.FullPath).ToArray();

                var unitTestElements = new UnitTestElements(SolutionCriterion.Instance, elements);

                elementManager.AddElements(elements.ToSet());

                var session = facade.SessionManager.CreateSession(SolutionCriterion.Instance);
                var launch = facade.LaunchManager.CreateLaunch(session, unitTestElements, UnitTestHost.Instance.GetProvider("Process"));

                launch.Run().Wait(lifetime);

                WriteResults(elements, output);
            });
        }

        private ICollection<IUnitTestElement> GetUnitTestElements(IProject testProject, string assemblyLocation)
        {
            var assembly = FileSystemPath.Parse(assemblyLocation);
            var observer = new TestUnitTestElementObserver(testProject, testProject.GetCurrentTargetFrameworkId(), assembly);

            MetadataExplorer.ProcessArtifact(observer, CancellationToken.None).Wait();

            return observer.Elements;
        }

        private void WriteResults(IUnitTestElement[] elements, TextWriter output)
        {
            var results = Solution.GetComponent<IUnitTestResultManager>();

            foreach (var element in elements)
            {
                var result = results.GetResult(element);

                output.WriteLine($"{element.Id.Provider.ID}::{element.Id.Project.GetPersistentID()}::{element.Id.Id}");
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
                    source.CopyFile(target, true);
                }
            }
        }
    }
}
