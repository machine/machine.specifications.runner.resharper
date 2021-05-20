using System.Threading;
using System.Threading.Tasks;
using JetBrains.ReSharper.TestFramework;
using JetBrains.ReSharper.UnitTestFramework.Extensions;
using JetBrains.ReSharper.UnitTestFramework.Processes;
using Microsoft.Win32.SafeHandles;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Runner
{
    public class FakeWaitHandle : WaitHandle
    {
    }

    [TestFixture]
    public class Fake
    {
        [Test]
        public void Test()
        {
            var source = new TaskCompletionSource<bool>();
            var handle = new ManualResetEvent(false);
            var waitHandle = new FakeWaitHandle
            {
                SafeWaitHandle = new SafeWaitHandle(handle.SafeWaitHandle.DangerousGetHandle(), false)
            };

            Task.Run(() =>
            {
                Thread.Sleep(5000);

                handle.Set();
                handle.Close();
                handle.Dispose();
            });

            var result = source.Task.ThrowIf(waitHandle, () =>
            {
                return new AbandonedMutexException();
            }).Result;
        }
    }

    [MspecReferences]
    [TestNetFramework46]
    public class MspecRunnerTests : MspecTaskRunnerTestBase
    {
        protected override string RelativeTestDataPath => "Runner";

        [Test]
        [ExcludeMsCorLib]
        public void SimpleSpec()
        {
            DoOneTest("SimpleSpec");
        }
    }
}
