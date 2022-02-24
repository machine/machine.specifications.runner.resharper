using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    public interface IMspecDiscoverySink
    {
        void OnSpecification(ISpecificationElement specification);

        void OnDiscoveryCompleted();
    }
}
