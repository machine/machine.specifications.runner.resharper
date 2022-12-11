using System;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution;

public class EmptyPreparedProcess : IPreparedProcess
{
    public int ProcessId => 0;

    public int ExitCode => 0;

    public IntPtr Handle => IntPtr.Zero;

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

    public void Start()
    {
    }

    public bool WaitForExit(TimeSpan? timeout = null)
    {
        return true;
    }

    public void Kill()
    {
    }
}
