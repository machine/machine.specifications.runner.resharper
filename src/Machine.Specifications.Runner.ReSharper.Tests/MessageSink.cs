using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Launch;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SolutionComponent]
    public class MessageSink : IMessageSink
    {
        public IUnitTestLaunchOutput Output { get; private set; }

        public void SetOutput(IUnitTestLaunchOutput output)
        {
            Output = output;
        }
    }
}
