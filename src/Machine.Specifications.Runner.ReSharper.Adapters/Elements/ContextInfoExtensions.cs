using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public static class ContextInfoExtensions
    {
        public static IContextElement ToElement(this ContextInfo context)
        {
            return new ContextElement(context.TypeName, context.Concern);
        }
    }
}
