using JetBrains.ReSharper.TestRunner.Abstractions;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

public class LoggingRunListener(IRunListener listener) : IRunListener
{
    private readonly ILogger logger = Logger.GetLogger<LoggingRunListener>();

    public void OnAssemblyStart(TestAssemblyInfo assemblyInfo)
    {
        logger.Trace($"OnAssemblyStart: {assemblyInfo.Location}");

        logger.Catch(() => listener.OnAssemblyStart(assemblyInfo));
    }

    public void OnAssemblyEnd(TestAssemblyInfo assemblyInfo)
    {
        logger.Trace($"OnAssemblyEnd: {assemblyInfo.Location}");

        logger.Catch(() => listener.OnAssemblyEnd(assemblyInfo));
    }

    public void OnRunStart()
    {
        logger.Trace("OnRunStart:");

        logger.Catch(listener.OnRunStart);
    }

    public void OnRunEnd()
    {
        logger.Trace("OnRunEnd:");

        logger.Catch(listener.OnRunEnd);
    }

    public void OnContextStart(TestContextInfo contextInfo)
    {
        logger.Trace($"OnContextStart: {contextInfo.TypeName}");

        logger.Catch(() => listener.OnContextStart(contextInfo));
    }

    public void OnContextEnd(TestContextInfo contextInfo)
    {
        logger.Trace($"OnContextEnd: {contextInfo.TypeName}");

        logger.Catch(() => listener.OnContextEnd(contextInfo));
    }

    public void OnSpecificationStart(TestSpecificationInfo specificationInfo)
    {
        logger.Trace($"OnSpecificationStart: {specificationInfo.ContainingType}.{specificationInfo.FieldName}");

        logger.Catch(() => listener.OnSpecificationStart(specificationInfo));
    }

    public void OnSpecificationEnd(TestSpecificationInfo specificationInfo, TestRunResult runResult)
    {
        logger.Trace($"OnSpecificationEnd: {specificationInfo.ContainingType}.{specificationInfo.FieldName}");

        logger.Catch(() => listener.OnSpecificationEnd(specificationInfo, runResult));
    }

    public void OnFatalError(TestError? error)
    {
        logger.Trace($"OnFatalError: {error?.FullTypeName}");

        logger.Catch(() => listener.OnFatalError(error));
    }
}
