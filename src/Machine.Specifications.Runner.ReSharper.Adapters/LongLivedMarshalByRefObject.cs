using System;
using System.Security;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
#if NETFRAMEWORK
    public abstract class LongLivedMarshalByRefObject : MarshalByRefObject
    {
        [SecurityCritical]
        public sealed override object InitializeLifetimeService() => null;
    }
#else
    public abstract class LongLivedMarshalByRefObject
    {
    }
#endif
}
