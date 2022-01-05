using Machine.Specifications.Runner.ReSharper.Adapters.Discovery.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    public interface IMspecDiscoverySink
    {
        void OnContext(Context context);

        void OnDiscoveryCompleted();
    }
}
