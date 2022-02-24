﻿using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    internal class AdapterListener : ISpecificationRunListener
    {
        private readonly IRunListener listener;

        private readonly string assemblyPath;

        public AdapterListener(IRunListener listener, string assemblyPath)
        {
            this.listener = listener;
            this.assemblyPath = assemblyPath;
        }

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            listener.OnAssemblyStart(new TestAssemblyInfo(assemblyPath));
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            listener.OnAssemblyEnd(assemblyInfo.ToTestAssembly());
        }

        public void OnRunStart()
        {
            listener.OnRunStart();
        }

        public void OnRunEnd()
        {
            listener.OnRunEnd();
        }

        public void OnContextStart(ContextInfo contextInfo)
        {
            listener.OnContextStart(contextInfo.ToTestContext());
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            listener.OnContextEnd(contextInfo.ToTestContext());
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            listener.OnSpecificationStart(specificationInfo.ToTestSpecification());
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            listener.OnSpecificationEnd(specificationInfo.ToTestSpecification(), result.ToTestResult());
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            listener.OnFatalError(exceptionResult.ToTestError());
        }
    }
}
