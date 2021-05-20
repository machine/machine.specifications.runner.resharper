using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Application.Processes;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.BindableLinq.Dependencies;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DotNetCore;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.TestFramework;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Extensions;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.Launch.Stages;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.Activation;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.DataCollection;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Machine.Specifications.Runner.ReSharper.Runner;
using Machine.Specifications.Runner.ReSharper.RunStrategies;
using Microsoft.CSharp;
using Microsoft.Win32.SafeHandles;
using ILogger = JetBrains.Util.ILogger;
using Logger = JetBrains.Util.Logging.Logger;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class ExecutionAgent : ITestRunnerExecutionAgent
    {
        public Task<int> Shutdown()
        {
            return Task.FromResult(0)
        }

        public string Id { get; }
        public Lifetime Lifetime { get; }
        public IPreparedProcess Process { get; }
        public IMessageBroker MessageBroker { get; }
        public object ActivationOptions { get; }
        public ITestRunnerExecutionContext Context { get; }
        public Task RunTests(CancellationToken cancelCt, CancellationToken abortCt)
        {
            throw new NotImplementedException();
        }
    }

    public class MspecRunnerAgentManager : ITestRunnerAgentManager
    {
        public Task<ITestRunnerExecutionAgent> GetExecutionAgent(ITestRunnerExecutionContext context, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<ITestRunnerDiscoveryAgent> GetDiscoveryAgent(ITestRunnerDiscoveryContext context, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }

    [SolutionComponent]
    public class MspecDifferentRunStrategy : MspecTestRunnerRunStrategy
    {
        public MspecDifferentRunStrategy(
            IDataCollectorFactory dataCollectorFactory,
            ITestRunnerHostSource testRunnerHostSource,
            MspecTestRunnerOrchestrator adapter,
            IUnitTestProjectArtifactResolver artifactResolver,
            DotNetCoreLaunchSettingsJsonProfileProvider launchSettingsProvider)
            : base(dataCollectorFactory, new MspecRunnerAgentManager(), testRunnerHostSource, adapter, artifactResolver, launchSettingsProvider)
        {
        }
    }

    public class MspecTestRunnerAgent : ITestRunnerAgent, IHideImplementation<MspecTestRunnerAgent>
    {
        public string Id { get; }

        public Lifetime Lifetime { get; }

        public IPreparedProcess Process { get; }

        public IMessageBroker MessageBroker { get; }

        public object ActivationOptions { get; }

        public Task<int> Shutdown()
        {
            throw new NotImplementedException();
        }
    }

    //[ShellComponent]
    public class MspecTestRunnerAgentInvoker : ITestRunnerAgentInvoker
    {
        public Task<ITestRunnerAgent> StartTestRunnerAgent(ITestRunnerContext ctx, CancellationToken ct)
        {
            return Task.FromResult<ITestRunnerAgent>(new MspecTestRunnerAgent());
        }
    }

    public class MspecProcess : IPreparedProcess
    {
        private AutoResetEvent handle = new AutoResetEvent(false);

        public void Dispose()
        {
        }

        public void Start()
        {
            IsRunning = true;

            Task.Run(() =>
            {
                Thread.Sleep(10000);

                OnExit();
            });
        }

        private void OnExit()
        {
            IsRunning = false;

            handle.Set();

            if (Exited != null)
            {
                Exited(this, 0);
            }
        }

        public bool WaitForExit(TimeSpan? timeout = null)
        {
            throw new NotImplementedException();
        }

        public void Kill()
        {
        }

        public int ProcessId { get; } = 5;

        public int ExitCode { get; }

        public IntPtr Handle => handle.SafeWaitHandle.DangerousGetHandle();

        public string ProcessName { get; } = "fake";

        public string ProcessArgs { get; } = string.Empty;

        public string Output { get; } = string.Empty;

        public bool IsRunning { get; set; }

        public DateTime? StartTime { get; }

        public DateTime? ExitTime { get; }

        public event ExitProcessHandler Exited;

        public event LineReadHandler OutputLineRead;

        public event LineReadHandler ErrorLineRead;
    }

    public class MspecTestRunnerHost : ITestRunnerHost
    {
        public int Priority { get; }

        public IEnumerable<Assembly> InProcessAssemblies => Array.Empty<Assembly>();

        public IEnumerable<IMessageHandlerMarker> GetMessageHandlers(ITestRunnerContext context)
        {
            yield break;
        }

        public IPreparedProcess StartProcess(ProcessStartInfo startInfo, ITestRunnerContext context)
        {
            return new MspecProcess();
        }
    }

    [SolutionComponent]
    public class MspecHostSource : ITestRunnerHostProvider
    {
        public ITestRunnerHost TryGetHost(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return new MspecTestRunnerHost();
        }
    }

    public class MspecHostController : RunHostController
    {
        public MspecHostController(IUnitTestLaunch launch, ISolutionProcessStartInfoPatcher processStartInfoPatcher, ILogger logger)
            : base(launch, processStartInfoPatcher, logger)
        {
        }

        public override IPreparedProcess StartProcess(ProcessStartInfo startInfo, IUnitTestRun run, ILogger logger)
        {
            return base.StartProcess(startInfo, run, logger);
        }
    }

    [UnitTestHostProvider]
    public class MspecTestHostProvider : RunHostProvider
    {
        public MspecTestHostProvider()
        {
        }

        public override ITaskRunnerHostController CreateHostController(IUnitTestLaunch launch)
        {
            return new MspecHostController(launch, launch.Solution.GetComponent<ISolutionProcessStartInfoPatcher>(), Logger.GetLogger<MspecHostController>());
        }
    }

    public class Adapter : ITestRunnerAgentManager
    {
        public Task<ITestRunnerExecutionAgent> GetExecutionAgent(ITestRunnerExecutionContext context, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<ITestRunnerDiscoveryAgent> GetDiscoveryAgent(ITestRunnerDiscoveryContext context, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }

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
            var evaluator = Solution.GetComponent<IUnitTestElementCriterionEvaluator>();
            var elementManager = Solution.GetComponent<IUnitTestElementManager>();
            var rules = Solution.GetComponents<IUnitTestElementsTransformationRule>();
            var strategy = Solution.GetComponent<MspecTestRunnerRunStrategy>();

            var projectFile = testProject.GetSubItems().First();

            var elements = GetUnitTestElements(testProject, projectFile.Location.FullPath).ToArray();

            elementManager.AddElements(elements.ToSet());

            var session = facade.SessionManager.CreateSession(new EverythingCriterion());

            var provider = UnitTestHost.Instance.GetProvider("Process");
            var unitTestElements = new UnitTestElements(new EverythingCriterion(), elements);

            var launch = facade.LaunchManager.CreateLaunch(session, unitTestElements, provider);

            launch.Output.Subscribe(new OutputObserver());

            launch.Elements.CollectionChanged += (sender, args) =>
            {
                var item = args.NewItems;
            };

            var stagesProvider = launch.GetComponent<IUnitTestLaunchStagesProvider>();
            var stages = stagesProvider.GetStages(launch);

            foreach (var stage in stages)
            {
                stage.Run(CancellationToken.None, CancellationToken.None).Wait();
            }

            launch.Run().Wait();
        }

        private void Rules()
        {

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
