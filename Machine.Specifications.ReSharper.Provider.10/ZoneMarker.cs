using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperProvider
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IUnitTestingZone>
    {
    }
}