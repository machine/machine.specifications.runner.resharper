namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    public class TestAssemblyInfo
    {
        public TestAssemblyInfo(string location)
        {
            Location = location;
        }

        public string Location { get; }
    }
}
