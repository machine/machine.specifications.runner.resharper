namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    public interface IMspecController
    {
        void Find(IMspecDiscoverySink sink, string assemblyPath);
    }
}
