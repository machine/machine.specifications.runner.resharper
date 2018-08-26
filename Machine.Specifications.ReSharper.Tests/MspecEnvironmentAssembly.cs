using JetBrains.TestFramework;
using NUnit.Framework;

[assembly: TestDataPathBase("Data")]
[assembly: RequiresSTA]

namespace Machine.Specifications.ReSharper.Tests
{
    [SetUpFixture]
    public class MspecEnvironmentAssembly : ExtensionTestEnvironmentAssembly<IMspecTestZone>
    {
    }
}
