using System;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecPreparedProcess : IPreparedProcess
    {
        public int ProcessId { get; }

        public int ExitCode { get; }

        public IntPtr Handle { get; }

        public string ProcessName { get; }

        public string ProcessArgs { get; }

        public string Output { get; }

        public bool IsRunning => false;

        public DateTime? StartTime { get; }

        public DateTime? ExitTime { get; }

        public event ExitProcessHandler Exited;

        public event LineReadHandler OutputLineRead;

        public event LineReadHandler ErrorLineRead;

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
