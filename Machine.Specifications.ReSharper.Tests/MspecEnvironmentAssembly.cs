using JetBrains.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    [SetUpFixture]
    public class MspecEnvironmentAssembly : ExtensionTestEnvironmentAssembly<IMspecTestZone>
    {
    }
}
