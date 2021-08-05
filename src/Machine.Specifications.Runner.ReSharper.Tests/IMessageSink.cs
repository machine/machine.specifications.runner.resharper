using JetBrains.ReSharper.UnitTestFramework.Launch;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public interface IMessageSink
    {
        IUnitTestLaunchOutput Output { get; }

        void SetOutput(IUnitTestLaunchOutput output);
    }
}
