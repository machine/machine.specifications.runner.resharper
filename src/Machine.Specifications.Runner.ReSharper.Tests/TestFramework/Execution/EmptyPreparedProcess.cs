using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Util;
using JetBrains.Util.Processes;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution;

public class EmptyPreparedProcess : IPreparedProcessWithCachedOutput
{
    public int ProcessId => 0;

    public int ExitCode => 0;

    public string ProcessName => string.Empty;

    public string ProcessArgs => string.Empty;

    public string Output => string.Empty;

    public bool IsRunning => false;

    public DateTime? StartTime { get; } = DateTime.Now;

    public DateTime? ExitTime { get; } = DateTime.Now;

    public event ExitProcessHandler Exited
    {
        add { }
        remove { }
    }

    public event LineReadHandler OutputLineRead
    {
        add { }
        remove { }
    }

    public event LineReadHandler ErrorLineRead
    {
        add { }
        remove { }
    }

    public void Dispose()
    {
    }

    public Task Start(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public bool WaitForExit(TimeSpan? timeout = null)
    {
        return true;
    }

    public void Kill()
    {
    }
}
