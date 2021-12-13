using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Models
{
    public class FrameworkContext : IContext
    {
        public FrameworkContext(ContextInfo context)
        {
            TypeName = context.TypeName;
            Subject = context.Concern;
            Name = context.Name;
        }

        public string TypeName { get; }

        public string Subject { get; }

        public string Name { get; }
    }
}
