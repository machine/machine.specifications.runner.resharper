namespace Machine.Specifications.Runner.ReSharper
{
    public interface ITaskIdentifiable
    {
        string GetId();

        bool IsContext();
    }
}
