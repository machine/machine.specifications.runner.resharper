using System;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecPreparedProcess : IPreparedProcess
    {
        public int ProcessId { get; } = 0;

        public int ExitCode { get; } = 0;

        public IntPtr Handle { get; } = IntPtr.Zero;

        public string ProcessName { get; } = string.Empty;

        public string ProcessArgs { get; } = string.Empty;

        public string Output { get; } = string.Empty;

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
}
