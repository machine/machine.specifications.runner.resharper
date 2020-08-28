using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class MspecTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Machine.Specifications";

        private readonly TestRunner testRunner;

        public MspecTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            testRunner = new TestRunner(server);
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var assemblyTask = node.RemoteTask as MspecAssemblyTask;

            if (assemblyTask == null)
            {
                return;
            }

            var context = new TestContext(assemblyTask.AssemblyLocation);

            PopulateContext(context, node);
            
            testRunner.Run(context);
        }

        private void PopulateContext(TestContext context, TaskExecutionNode node)
        {
            var childNodes = node.Children.Flatten(x => x.Children);

            foreach (var childNode in childNodes)
            {
                switch (childNode.RemoteTask)
                {
                    case MspecContextTask task:
                        context.AddContext(task.GetId(), task);
                        break;

                    case MspecBehaviorSpecificationTask task:
                        context.AddSpecification(task.GetId(), task);
                        break;

                    case MspecContextSpecificationTask task:
                        context.AddSpecification(task.GetId(), task);
                        break;
                }
            }
        }
    }
}
