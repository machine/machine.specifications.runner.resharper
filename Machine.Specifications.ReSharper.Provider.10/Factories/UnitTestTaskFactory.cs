using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Presentation;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    public class UnitTestTaskFactory
    {
        public UnitTestTask CreateRunAssemblyTask(ContextElement context, UnitTestElementId id)
        {
            return new UnitTestTask(null, new MspecTestAssemblyTask(id.ProjectId, context.AssemblyLocation));
        }

        public UnitTestTask CreateContextTask(ContextElement context, UnitTestElementId id)
        {
            return new UnitTestTask(context,
                new MspecTestContextTask(id.ProjectId,
                    context.GetTypeClrName().FullName));
        }

        public UnitTestTask CreateContextSpecificationTask(ContextElement context, ContextSpecificationElement contextSpecification, UnitTestElementId id)
        {
            return new UnitTestTask(contextSpecification,
                new MspecTestSpecificationTask(id.ProjectId,
                    context.GetTypeClrName().FullName,
                    contextSpecification.FieldName));
        }

        public UnitTestTask CreateBehaviorSpecificationTask(ContextElement context, BehaviorSpecificationElement behaviorSpecification, UnitTestElementId id)
        {
            return new UnitTestTask(behaviorSpecification,
                new MspecTestBehaviorTask(
                    id.ProjectId,
                    context.GetTypeClrName().FullName,
                    behaviorSpecification.Behavior.FieldName,
                    behaviorSpecification.FieldName,
                    behaviorSpecification.Behavior.FieldType));
        }
    }
}