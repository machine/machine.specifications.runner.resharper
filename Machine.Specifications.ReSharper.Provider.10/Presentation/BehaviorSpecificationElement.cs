using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.Util;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class BehaviorSpecificationElement : FieldElement
    {
        public BehaviorSpecificationElement(
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored)
            : base(id, parent, typeName, serviceProvider, fieldName, isIgnored || parent.Explicit)
        {
        }

        public BehaviorElement Behavior => (BehaviorElement)Parent;

        public override string Kind => "Behavior Specification";

        public override UnitTestElementDisposition GetDisposition()
        {
            var valid = Behavior?.GetDeclaredElement()?.IsValid();

            if (!valid.GetValueOrDefault())
                return UnitTestElementDisposition.InvalidDisposition;

            return base.GetDisposition();
        }

        public override IEnumerable<UnitTestElementLocation> GetLocations(IDeclaredElement element)
        {
            return Behavior.GetLocations(element);
        }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var context = Behavior.Context;

            return new List<UnitTestTask>
            {
                new UnitTestTask(null, new MspecTestAssemblyTask(Id.ProjectId, context.AssemblyLocation.FullPath)),
                new UnitTestTask(context, new MspecTestContextTask(Id.ProjectId, context.TypeName.FullName)),
                new UnitTestTask(this, new MspecTestBehaviorTask(Id.ProjectId, context.TypeName.FullName, Behavior.FieldName, FieldName, Behavior.FieldType))
            };
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, IUnitTestProvider provider, BehaviorElement behaviorElement, string fieldName)
        {
            var result = new[] { behaviorElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, behaviorElement.Id.Project, consumer.TargetFrameworkId, id);
        }
    }
}
