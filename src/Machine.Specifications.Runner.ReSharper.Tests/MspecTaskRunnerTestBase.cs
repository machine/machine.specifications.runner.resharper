using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Application.Settings;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Runner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public abstract class MspecTaskRunnerTestBase : UnitTestTaskRunnerTestBase
    {
        public override IUnitTestExplorerFromArtifacts MetadataExplorer => Solution.GetComponent<MspecTestExplorerFromArtifacts>();

        protected override void ChangeSettingsForTest(IContextBoundSettingsStoreLive settingsStore)
        {
        }

        protected override RemoteTaskRunnerInfo GetRemoteTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(MspecTaskRunner.RunnerId, typeof(MspecTaskRunner));
        }

        protected override ICollection<IUnitTestElement> GetUnitTestElements(IProject testProject, string assemblyLocation)
        {
            var assembly = FileSystemPath.Parse(assemblyLocation);
            var observer = new TestUnitTestElementObserver(testProject, testProject.GetCurrentTargetFrameworkId(), assembly);

            MetadataExplorer.ProcessArtifact(observer, CancellationToken.None).Wait();

            return observer.Elements;
        }

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
