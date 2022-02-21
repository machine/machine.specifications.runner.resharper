using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public interface IExecutionListener
    {
        void OnAssemblyStart(string assemblyLocation);

        void OnAssemblyEnd(string assemblyLocation);

        void OnRunStart();

        void OnRunEnd();

        void OnContextStart(IContextElement context);

        void OnContextEnd(IContextElement context, string capturedOutput);

        void OnBehaviorStart(IBehaviorElement behavior);

        void OnBehaviorEnd(IBehaviorElement behavior, string capturedOutput);

        void OnSpecificationStart(ISpecificationElement specification);

        void OnSpecificationEnd(ISpecificationElement specification, string capturedOutput, TestRunResult runResult);

        void OnFatalError(TestError error);
    }
}
