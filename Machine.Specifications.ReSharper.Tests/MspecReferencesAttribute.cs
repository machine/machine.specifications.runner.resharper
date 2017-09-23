using JetBrains.ReSharper.TestFramework;

namespace Machine.Specifications.ReSharper.Tests
{
    public class MspecReferencesAttribute : TestPackagesAttribute
    {
        public MspecReferencesAttribute()
            : base("Machine.Specifications/0.11.0", "Machine.Specifications.Should/0.11.0")
        {
        }
    }
}
