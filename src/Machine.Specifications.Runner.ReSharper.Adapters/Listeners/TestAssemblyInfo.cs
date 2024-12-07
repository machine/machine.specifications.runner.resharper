namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

public class TestAssemblyInfo(string location)
{
    public string Location { get; } = location;
}
