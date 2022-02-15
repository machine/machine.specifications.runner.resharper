using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class SpecificationAdapterListener : ISpecificationRunListener
    {
        private readonly IExecutionListener listener;

        private readonly ElementCache cache;

        private readonly RunTracker tracker;

        private readonly HashSet<IMspecElement> behaviors = new();

        private IContextElement? currentContext;

        public SpecificationAdapterListener(IExecutionListener listener, ElementCache cache, RunTracker tracker)
        {
            this.listener = listener;
            this.cache = cache;
            this.tracker = tracker;
        }

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
        }

        public void OnContextStart(ContextInfo contextInfo)
        {
            var context = tracker.StartContext(contextInfo.TypeName);

            currentContext = context;

            if (context != null)
            {
                listener.OnContextStart(context);
            }
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            var context = tracker.FinishContext(contextInfo.TypeName);

            if (context != null)
            {
                var runningBehaviors = cache.GetBehaviors(context)
                    .Where(x => behaviors.Contains(x));

                foreach (var behavior in runningBehaviors)
                {
                    listener.OnBehaviorEnd(behavior);
                }

                listener.OnContextEnd(context);
            }

            currentContext = null;
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            var isBehavior = cache.IsBehavior(specificationInfo.ContainingType);

            if (isBehavior)
            {
                var key = $"{currentContext?.TypeName}.{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

                var specification = tracker.StartSpecification(key);

                if (specification?.Behavior != null && behaviors.Add(specification.Behavior))
                {
                    listener.OnBehaviorStart(specification.Behavior);
                }

                if (specification != null)
                {
                    listener.OnSpecificationStart(specification);
                }
            }
            else
            {
                var key = $"{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

                var specification = tracker.StartSpecification(key);

                if (specification != null)
                {
                    listener.OnSpecificationStart(specification);
                }
            }
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            var isBehavior = cache.IsBehavior(specificationInfo.ContainingType);

            if (isBehavior)
            {
                var key = $"{currentContext?.TypeName}.{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

                var specification = tracker.FinishSpecification(key);

                if (specification != null)
                {
                    listener.OnSpecificationEnd(specification, result);
                }
            }
            else
            {
                var key = $"{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

                var specification = tracker.FinishSpecification(key);

                if (specification != null)
                {
                    listener.OnSpecificationEnd(specification, result);
                }
            }
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            listener.OnFatalError(exceptionResult);
        }
    }
}
