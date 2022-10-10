using JetBrains.ReSharper.TestFramework;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework
{
    public class MspecReferencesAttribute : TestPackagesAttribute
    {
        public MspecReferencesAttribute()
            : base("Machine.Specifications/1.1.0", "Machine.Specifications.Should/1.0.0")
        {
        }
    }
}
