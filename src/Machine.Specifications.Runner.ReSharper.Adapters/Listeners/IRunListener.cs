namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

public interface IRunListener
{
    void OnAssemblyStart(TestAssemblyInfo assemblyInfo);

    void OnAssemblyEnd(TestAssemblyInfo assemblyInfo);

    void OnRunStart();

    void OnRunEnd();

    void OnContextStart(TestContextInfo contextInfo);

    void OnContextEnd(TestContextInfo contextInfo);

    void OnSpecificationStart(TestSpecificationInfo specificationInfo);

    void OnSpecificationEnd(TestSpecificationInfo specificationInfo, TestRunResult runResult);

    void OnFatalError(TestError? error);
}
