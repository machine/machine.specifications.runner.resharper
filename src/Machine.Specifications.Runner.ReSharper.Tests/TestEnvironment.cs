using System.Threading;
using JetBrains.TestFramework;
using NUnit.Framework;

[assembly: RequiresThread(ApartmentState.STA)]

#pragma warning disable CS0618
[assembly: TestDataPathBase("Data")]
#pragma warning restore CS0618

namespace Machine.Specifications.Runner.ReSharper.Tests;

[SetUpFixture]
public class TestEnvironment : ExtensionTestEnvironmentAssembly<IMspecTestZone>
{
}
