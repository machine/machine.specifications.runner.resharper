using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Application.Zones;

namespace Machine.Specifications.ReSharper.Tests
{
    [ZoneDefinition]
    public interface IMspecTestZone : ITestsZone, IRequire<PsiFeatureTestZone>
    {
    }
}