using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class TestRunListener : ISpecificationRunListener
    {
        private readonly IExecutionListener listener;

        private readonly ResultsContainer container;

        private readonly ManualResetEvent waitEvent = new(false);

        private readonly HashSet<IBehaviorElement> currentBehaviors = new();

        private IContextElement? currentContext;

        public TestRunListener(IExecutionListener listener, ResultsContainer container)
        {
            this.listener = listener;
            this.container = container;
        }

        public WaitHandle Finished => waitEvent;

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            listener.OnAssemblyStart(assemblyInfo.Location);
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            listener.OnAssemblyEnd(assemblyInfo.Location);
        }

        public void OnRunStart()
        {
            listener.OnRunStart();
        }

        public void OnRunEnd()
        {
            listener.OnRunEnd();

            waitEvent.Set();
        }

        public void OnContextStart(ContextInfo contextInfo)
        {
            var context = container.Started<IContextElement>(contextInfo.TypeName);

            if (context != null)
            {
                currentContext = context;

                listener.OnContextStart(context);
            }
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            var result = container.Get(contextInfo.TypeName);

            result.Finished(result.IsSuccessful);

            currentContext = null;
            currentBehaviors.Clear();

            listener.OnContextEnd((IContextElement) result.Element);
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            var isBehavior = container.IsBehavior(specificationInfo.ContainingType);

            if (isBehavior)
            {
                var key = $"{currentContext!.TypeName}.{specificationInfo.ContainingType}";

                var behavior = currentBehaviors.FirstOrDefault(x => x.TypeName == specificationInfo.ContainingType) ??
                               container.Started<IBehaviorElement>(key);

                if (behavior != null && currentBehaviors.Add(behavior))
                {
                    listener.OnBehaviorStart(behavior);
                }

                var specification = container.Started<ISpecificationElement>($"{key}.{specificationInfo.FieldName}");

                if (specification != null)
                {
                    listener.OnSpecificationStart(specification);
                }
            }
            else
            {
                var key = $"{specificationInfo.ContainingType}.{specificationInfo.FieldName}";
                var specification = container.Started<ISpecificationElement>(key);

                if (specification != null)
                {
                    listener.OnSpecificationStart(specification);
                }
            }
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            listener.OnFatalError(exceptionResult);
        }
    }
}
