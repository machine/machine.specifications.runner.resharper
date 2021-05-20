using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Session;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Runner;
using Microsoft.CSharp;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecLegacyTaskRunnerTestBase : UnitTestTaskRunnerTestBase
    {
        public override IUnitTestExplorerFromArtifacts MetadataExplorer => Solution.GetComponent<MspecTestExplorerFromArtifacts>();

        protected override void ChangeSettingsForTest(IContextBoundSettingsStoreLive settingsStore)
        {
        }

        protected override RemoteTaskRunnerInfo GetRemoteTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(MspecTaskRunner.RunnerId, typeof(MspecTaskRunner));
        }

        protected override void DoTest(Lifetime lifetime, IProject testProject)
        {
            var facade = Solution.GetComponent<IUnitTestingFacade>();

            var assembly = GetDll(testProject.ProjectFile?.Location);

            var elements = GetUnitTestElements(testProject, assembly);

            var session = facade.SessionManager.CreateSession(SolutionCriterion.Instance);

            DoTest(testProject.ProjectFile, session, null, lifetime);
        }

        protected override void DoExecute(IUnitTestSession session, List<IList<UnitTestTask>> sequences, Lifetime lt, TextWriter output)
        {
            var controller = CreateTaskRunnerHostController(
                Solution.GetComponent<IUnitTestResultManager>(),
                Solution.GetComponent<IUnitTestAgentManager>(),
                output,
                session.Launch.Value,
                Solution.GetComponent<IRemoteChannelMessageListener>());

            var info = GetRemoteTaskRunnerInfo();
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
    }
}
