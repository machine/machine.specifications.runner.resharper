using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner
{
    public class MspecTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "MSpec.Specifications.Rob";

        private readonly TestRunner _testRunner;

        public MspecTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            _testRunner = new TestRunner(server);
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var assemblyTask = node.RemoteTask as MspecTestAssemblyTask;

            if (assemblyTask == null)
                return;

            var context = new TestContext(assemblyTask);

            PopulateContext(context, node);
            
            _testRunner.Run(context);
        }

        private void PopulateContext(TestContext context, TaskExecutionNode node)
        {
            var childNodes = node.Children.Flatten(x => x.Children);

            foreach (var childNode in childNodes)
            {
                switch (childNode.RemoteTask)
                {
                    case MspecTestContextTask task:
                        context.Add(task);
                        break;

                    case MspecTestBehaviorTask task:
                        context.Add(task);
                        break;

                    case MspecTestSpecificationTask task:
                        context.Add(task);
                        break;
                }
            }
        }
    }
}