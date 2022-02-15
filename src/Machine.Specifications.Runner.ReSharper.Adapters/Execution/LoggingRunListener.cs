using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class LoggingRunListener : ISpecificationRunListener
    {
        private readonly ISpecificationRunListener listener;

        private readonly ILogger logger = Logger.GetLogger<LoggingRunListener>();

        private readonly ManualResetEvent waitEvent = new(false);

        public LoggingRunListener(ISpecificationRunListener listener)
        {
            this.listener = listener;
        }

        public WaitHandle Finished => waitEvent;

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            logger.Trace($"OnAssemblyStart: {assemblyInfo.Location}");

            logger.Catch(() => listener.OnAssemblyStart(assemblyInfo));
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            logger.Trace($"OnAssemblyEnd: {assemblyInfo.Location}");

            logger.Catch(() => listener.OnAssemblyEnd(assemblyInfo));
        }

        public void OnRunStart()
        {
            logger.Trace("OnRunStart:");

            logger.Catch(() => listener.OnRunStart());
        }

        public void OnRunEnd()
        {
            logger.Trace("OnRunEnd:");

            logger.Catch(() => listener.OnRunEnd());

            waitEvent.Set();
        }

        public void OnContextStart(ContextInfo contextInfo)
        {
            logger.Trace($"OnContextStart: {contextInfo.TypeName}");

            logger.Catch(() => listener.OnContextStart(contextInfo));
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            logger.Trace($"OnContextEnd: {contextInfo.TypeName}");

            logger.Catch(() => listener.OnContextEnd(contextInfo));
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            logger.Trace($"OnSpecificationStart: {specificationInfo.ContainingType}.{specificationInfo.FieldName}");

            logger.Catch(() => listener.OnSpecificationStart(specificationInfo));
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            logger.Trace($"OnSpecificationEnd: {specificationInfo.ContainingType}.{specificationInfo.FieldName}");

            logger.Catch(() => listener.OnSpecificationEnd(specificationInfo, result));
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            logger.Trace($"OnFatalError: {exceptionResult.FullTypeName}");

            logger.Catch(() => listener.OnFatalError(exceptionResult));
        }
    }
}
