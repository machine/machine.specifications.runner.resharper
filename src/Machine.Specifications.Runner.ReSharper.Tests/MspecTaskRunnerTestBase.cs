using System;
using System.CodeDom.Compiler;
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
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Runner;
using Microsoft.CSharp;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public abstract class MspecTaskRunnerTestBase : UnitTestTaskRunnerTestBase
    {
        private static readonly string[] FrameworkAssemblies =
        {
            "Machine.Specifications.dll",
            "Machine.Specifications.Should.dll"
        };

        public override IUnitTestExplorerFromArtifacts MetadataExplorer => Solution.GetComponent<MspecTestExplorerFromArtifacts>();

        protected override void ChangeSettingsForTest(IContextBoundSettingsStoreLive settingsStore)
        {
        }

        protected override RemoteTaskRunnerInfo GetRemoteTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(MspecTaskRunner.RunnerId, typeof(MspecTaskRunner));
        }

        protected override void DoOneTest(string testName)
        {
            var path = GetTestDataFilePath2(testName + Extension);

            CopyFrameworkLibrary(path.Directory);

            var assembly = GetDll(path);

            DoTestSolution(assembly);
        }

        protected override ICollection<IUnitTestElement> GetUnitTestElements(IProject testProject, string assemblyLocation)
        {
            var assembly = FileSystemPath.Parse(assemblyLocation);
            var observer = new TestUnitTestElementObserver(testProject, testProject.GetCurrentTargetFrameworkId(), assembly);

            MetadataExplorer.ProcessArtifact(observer, CancellationToken.None).Wait();

            return observer.Elements;
        }

        protected override void DoTest(Lifetime lifetime, IProject testProject)
        {
            var facade = Solution.GetComponent<IUnitTestingFacade>();
            var elementManager = Solution.GetComponent<IUnitTestElementManager>();

            var projectFile = testProject.GetSubItems().First();

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

                output.WriteLine(element.Id.ToString());
                output.Write("  ");
                output.WriteLine(result.ToString());
            }
        }

        private string GetDll(FileSystemPath source)
        {
            var assembly = source.ChangeExtension("dll");

            if (assembly.ExistsFile && source.FileModificationTimeUtc <= assembly.FileModificationTimeUtc)
            {
                return assembly.Name;
            }

            var references = GetReferencedAssemblies(GetTargetFrameworkId())
                .Where(Path.IsPathRooted)
                .ToArray();

            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.AddRange(references);
            parameters.OutputAssembly = assembly.ToString();

            var provider = new CSharpCodeProvider();
            var result = provider.CompileAssemblyFromFile(parameters, source.ToString());

            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors.ToStringWithCount());
            }

            return assembly.Name;
        }

        private void CopyFrameworkLibrary(FileSystemPath destination)
        {
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var frameworkAssembly in FrameworkAssemblies)
            {
                var source = assembly.GetPath().Directory.Combine(frameworkAssembly);
                var target = destination.Combine(frameworkAssembly);

                source.CopyFile(target, true);
            }
        }
    }
}
