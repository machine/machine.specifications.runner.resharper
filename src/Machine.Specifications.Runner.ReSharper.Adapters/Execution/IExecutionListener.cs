using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public interface IExecutionListener
    {
        void OnAssemblyStart(string assemblyLocation);

        void OnAssemblyEnd(string assemblyLocation);

        void OnRunStart();

        void OnRunEnd();

        void OnContextStart(IContextElement context);

        void OnContextEnd(IContextElement context);

        void OnBehaviorStart(IBehaviorElement behavior);

        void OnBehaviorEnd(IBehaviorElement behavior);

        void OnSpecificationStart(ISpecificationElement specification);

        void OnSpecificationEnd(ISpecificationElement specification, Result result);

        void OnFatalError(ExceptionResult exceptionResult);
    }
}
