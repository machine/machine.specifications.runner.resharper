using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class TestRunListener : ISpecificationRunListener
    {
        private readonly IExecutionListener listener;

        private readonly ElementCache cache;

        private readonly RunTracker tracker;

        private readonly ILogger logger = Logger.GetLogger<TestRunListener>();

        private readonly ManualResetEvent waitEvent = new(false);

        private readonly HashSet<IMspecElement> behaviors = new();

        private IContextElement? currentContext;

        public TestRunListener(IExecutionListener listener, ElementCache cache, RunTracker tracker)
        {
            this.listener = listener;
            this.cache = cache;
            this.tracker = tracker;
        }

        public WaitHandle Finished => waitEvent;

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            logger.Trace($"OnAssemblyStart: {assemblyInfo.Location}");

            logger.Catch(() => listener.OnAssemblyStart(assemblyInfo.Location));
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            logger.Trace($"OnAssemblyEnd: {assemblyInfo.Location}");

            logger.Catch(() => listener.OnAssemblyEnd(assemblyInfo.Location));
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

            logger.Catch(() =>
            {
                var context = tracker.StartContext(contextInfo.TypeName);

                currentContext = context;

                if (context != null)
                {
                    listener.OnContextStart(context);
                }
            });
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            logger.Trace($"OnContextEnd: {contextInfo.TypeName}");

            logger.Catch(() =>
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
            });
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            logger.Trace($"OnSpecificationStart: {specificationInfo.ContainingType}.{specificationInfo.FieldName}");

            logger.Catch(() =>
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
            });
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            logger.Trace($"OnSpecificationEnd: {specificationInfo.ContainingType}.{specificationInfo.FieldName}");

            logger.Catch(() =>
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
            });
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            logger.Trace($"OnFatalError: {exceptionResult.FullTypeName}");

            logger.Catch(() => listener.OnFatalError(exceptionResult));
        }
    }
}
