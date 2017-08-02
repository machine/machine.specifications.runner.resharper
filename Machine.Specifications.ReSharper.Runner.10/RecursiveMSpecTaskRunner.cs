using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Notifications;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner
{
    public class RecursiveMSpecTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Machine.Specifications";

        private static readonly RemoteTaskNotificationFactory TaskNotificationFactory = new RemoteTaskNotificationFactory();

        public RecursiveMSpecTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var task = (RunAssemblyTask)node.RemoteTask;

            var priorCurrentDirectory = Environment.CurrentDirectory;
            try
            {
                // Use the assembly in the folder that the user has specified, or, if not, use the assembly location
                var assemblyFolder = GetAssemblyFolder(TaskExecutor.Configuration, task);
                var assemblyPath = new AssemblyPath(Path.Combine(assemblyFolder, GetFileName(task.AssemblyLocation)));

                Environment.CurrentDirectory = assemblyFolder;

                var listener = new PerAssemblyRunListener(Server, task);
                var contextList = new List<string>();
                node.Flatten(x => x.Children).Each(children => RegisterRemoteTaskNotifications(listener, children, contextList));

                var runOptions = RunOptions.Custom.FilterBy(contextList);
                var appDomainRunner = new AppDomainRunner(listener, runOptions);

                if (TaskExecutor.Configuration.ShadowCopy)
                {
                    string cachePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                    runOptions.ShadowCopyTo(cachePath);
                    Server.SetTempFolderPath(cachePath);
                }

                appDomainRunner.RunAssembly(assemblyPath);
            }
            finally
            {
                Environment.CurrentDirectory = priorCurrentDirectory;
            }
        }

        private void RegisterRemoteTaskNotifications(PerAssemblyRunListener listener, TaskExecutionNode node, ICollection<string> contexts)
        {
            listener.RegisterTaskNotification(TaskNotificationFactory.CreateTaskNotification(node, contexts));
        }

        private static string GetAssemblyFolder(TaskExecutorConfiguration config, RunAssemblyTask assemblyTask)
        {
            return string.IsNullOrEmpty(config.AssemblyFolder)
                       ? GetDirectoryName(assemblyTask.AssemblyLocation)
                       : config.AssemblyFolder;
        }

        private static string GetDirectoryName(string filepath)
        {
            return Path.GetDirectoryName(filepath);
        }

        private static string GetFileName(string filepath)
        {
            return Path.GetFileName(filepath);
        }
    }
}