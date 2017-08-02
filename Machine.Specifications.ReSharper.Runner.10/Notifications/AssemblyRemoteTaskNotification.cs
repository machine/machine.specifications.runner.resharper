using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner.Notifications
{
    public class AssemblyRemoteTaskNotification : RemoteTaskNotification
    {
        private readonly TaskExecutionNode _node;
        private readonly RunAssemblyTask _task;

        public AssemblyRemoteTaskNotification(TaskExecutionNode node)
        {
            _node = node;
            _task = (RunAssemblyTask)node.RemoteTask;
        }

        private string Location => _task.AssemblyLocation;

        public override IEnumerable<RemoteTask> RemoteTasks
        {
            get { yield return _node.RemoteTask; }
        }

        public override bool Matches(object infoFromRunner, object maybeContext)
        {
            var assembly = infoFromRunner as Runner.Utility.AssemblyInfo;

            if (assembly == null)
                return false;

            return Location == assembly.Location;
        }

        public override string ToString()
        {
            return $"Assembly {Location} with {RemoteTasks.Count()} remote tasks";
        }
    }
}