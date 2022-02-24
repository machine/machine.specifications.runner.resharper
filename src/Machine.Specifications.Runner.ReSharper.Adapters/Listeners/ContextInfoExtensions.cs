using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    internal static class ContextInfoExtensions
    {
        public static TestContextInfo ToTestContext(this ContextInfo context)
        {
            return new TestContextInfo(context.TypeName, context.CapturedOutput);
        }
    }
}
