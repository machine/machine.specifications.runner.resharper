using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.BindableLinq.Dependencies;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Runner;
using Microsoft.CSharp;

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

        protected override void DoOneTest(string testName)
        {
            var path = GetTestDataFilePath2(testName + Extension);

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

        protected override ITaskRunnerHostController CreateTaskRunnerHostController(
            IUnitTestResultManager resultManager,
            IUnitTestAgentManager agentManager,
            TextWriter output,
            IUnitTestLaunch launch,
            IRemoteChannelMessageListener msgListener)
        {
            var controller = base.CreateTaskRunnerHostController(resultManager, agentManager, output, launch, msgListener);

            return controller;
        }

        protected override void DoTest(Lifetime lifetime, IProject testProject)
        {
            var facade = Solution.GetComponent<IUnitTestingFacade>();
            var elementManager = Solution.GetComponent<IUnitTestElementManager>();

            var projectFile = testProject.GetSubItems().First();

            var elements = GetUnitTestElements(testProject, projectFile.Location.FullPath).ToArray();

            elementManager.AddElements(elements.ToSet());

            var session = facade.SessionManager.CreateSession(new EverythingCriterion());

            var provider = UnitTestHost.Instance.GetProvider("Process");
            var unitTestElements = new UnitTestElements(new EverythingCriterion(), elements);

            var launch = facade.LaunchManager.CreateLaunch(session, unitTestElements, provider);

            launch.Output.Subscribe(new OutputObserver());

            launch.Run().Wait(lifetime);
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

        private class EverythingCriterion : IUnitTestElementCriterion
        {
            public IEnumerable<IDependencyDefinition> Dependencies { get; } = Array.Empty<IDependencyDefinition>();

            public bool Matches(IUnitTestElement element)
            {
                return true;
            }
        }

        private class OutputObserver : IObserver<IUnitTestLaunchOutputMessage>
        {
            public void OnNext(IUnitTestLaunchOutputMessage value)
            {
                var msg = value;
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }
        }
    }
}
