using Machine.Specifications.Runner.ReSharper.Adapters.Discovery.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Models
{
    public class TestElementContext : IContext
    {
        public TestElementContext(Context context)
        {
            TypeName = context.TypeName;
            Subject = context.Subject;
            Name = context.Name;
        }

        public string TypeName { get; }

        public string Subject { get; }

        public string Name { get; }
    }
}
