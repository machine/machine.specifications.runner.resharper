using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Application.Zones;

namespace Machine.Specifications.Runner.ReSharper.Tests;

[ZoneDefinition]
public interface IMspecTestZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>
{
}
