using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper
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
            var assemblyTask = node.RemoteTask as MspecTestAssemblyTask;

            if (assemblyTask == null)
            {
                return;
            }

            var context = new TestContext<RemoteTask>(assemblyTask.AssemblyLocation);

            PopulateContext(context, node);
            
            testRunner.Run(context);
        }

        private void PopulateContext(TestContext<RemoteTask> context, TaskExecutionNode node)
        {
            var childNodes = node.Children.Flatten(x => x.Children);

            foreach (var childNode in childNodes)
            {
                switch (childNode.RemoteTask)
                {
                    case MspecTestContextTask task:
                        context.AddContext(task.GetId(), task);
                        break;

                    case MspecTestBehaviorTask task:
                        context.AddSpecification(task.GetId(), task);
                        break;

                    case MspecTestSpecificationTask task:
                        context.AddSpecification(task.GetId(), task);
                        break;
                }
            }
        }
    }
}
