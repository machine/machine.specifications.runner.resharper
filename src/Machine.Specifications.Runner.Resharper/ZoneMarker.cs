using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.Runner.ReSharper
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IUnitTestingZone>
    {
    }
}
