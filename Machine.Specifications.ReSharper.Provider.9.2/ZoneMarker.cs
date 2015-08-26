using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperProvider
{
    // TODO: Ensure all used zones are correctly required (this really needs custom diagnostics)
    [ZoneMarker]
    public class ZoneMarker : IRequire<IUnitTestingZone>
    {         
    }
}