namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public interface IExecutionListener
    {
        void OnAssemblyStart(string assemblyLocation);

        void OnAssemblyEnd(string assemblyLocation);

        void OnRunStart();

        void OnRunEnd();

        void OnContextStart(string contextTypeName);

        void OnContextEnd(string contextTypeName);

        void OnSpecificationStart(string containingType, string fieldName);

        void OnSpecificationEnd(string containingType, string fieldName);

        void OnError();
    }
}
