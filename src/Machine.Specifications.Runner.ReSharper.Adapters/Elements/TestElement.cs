using System.Reflection;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public abstract class TestElement
    {
        public Assembly Assembly { get; set; }

        public string Name { get; set; }
    }
}
