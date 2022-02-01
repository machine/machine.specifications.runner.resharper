using System;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework
{
    public class LoggerFactory : ILoggerFactory
    {
        public ILogger GetLogger(string category)
        {
            return new Logger();
        }

        public void Setup(AppDomain appDomain, LoggerFactoryAppointment appointment)
        {
        }

        private class Logger : ILogger
        {
            public bool IsEnabled(LoggingLevel level)
            {
                return true;
            }

            public void Log(LoggingLevel level, string message, params object[] args)
            {
            }

            public void Log(LoggingLevel level, Exception exception, string message, params object[] args)
            {
            }

            public void Log(LoggingLevel level, LogMessageGenerator messageFunc)
            {
            }
        }
    }
}
