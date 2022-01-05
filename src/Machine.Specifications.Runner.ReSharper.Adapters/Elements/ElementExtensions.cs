using Machine.Specifications.Runner.ReSharper.Adapters.Discovery.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Models
{
    public static class ElementExtensions
    {
        public static IContext AsContext(this ContextInfo context)
        {
            return new FrameworkContext(context);
        }

        public static IContext AsContext(this Context context)
        {
            return new TestElementContext(context);
        }

        public static IContextSpecification AsSpecification(this ContextInfo context, SpecificationInfo specification)
        {
            return new FrameworkContextSpecification(context, specification);
        }

        public static IContextSpecification AsSpecification(this Specification specification)
        {
            return new TestElementContextSpecification(specification);
        }
    }
}
