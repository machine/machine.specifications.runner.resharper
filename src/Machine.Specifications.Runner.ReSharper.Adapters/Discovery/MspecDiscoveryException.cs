using System;
using System.Runtime.Serialization;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    [Serializable]
    public class MspecDiscoveryException : Exception
    {
        public MspecDiscoveryException(string message, string stackTrace)
            : base(message)
        {
            StackTrace = stackTrace;
        }

        protected MspecDiscoveryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StackTrace = info.GetString(nameof(StackTrace));
        }

        public override string StackTrace { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(StackTrace), StackTrace);
        }
    }
}
