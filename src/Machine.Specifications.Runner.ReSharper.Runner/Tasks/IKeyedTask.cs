using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    public interface IKeyedTask
    {
        string GetKey();

        RemoteTask AsRemoteTask();
    }
}
