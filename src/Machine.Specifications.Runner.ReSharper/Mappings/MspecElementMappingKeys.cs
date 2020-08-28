using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    public static class MspecElementMappingKeys
    {
        public static readonly Key<UnitTestElementFactory> ElementFactoryKey = new Key<UnitTestElementFactory>("MspecElementMapping.MspecElementFactory");
    }
}
