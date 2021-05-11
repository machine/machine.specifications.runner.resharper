using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Runner;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class BehaviorSpecificationElement : FieldElement, IEquatable<BehaviorSpecificationElement>
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

        public BehaviorElement Behavior => Parent as BehaviorElement;

        public override string Kind => "Behavior Specification";

        public override UnitTestElementDisposition GetDisposition()
        {
            var valid = Behavior?.GetDeclaredElement()?.IsValid();

            return valid.GetValueOrDefault()
                ? base.GetDisposition()
                : UnitTestElementDisposition.InvalidDisposition;
        }

        public override IEnumerable<UnitTestElementLocation> GetLocations(IDeclaredElement element)
        {
            return Behavior.GetLocations(element);
        }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var sequence = Behavior.GetTaskSequence(explicitElements, run);

            var behaviorSpecificationTask = run.GetRemoteTaskForElement<MspecBehaviorSpecificationRunnerTask>(this) ??
                                            new MspecBehaviorSpecificationRunnerTask(Id.ProjectId, Behavior.Context.TypeName.FullName, Behavior.FieldName, FieldName);

            sequence.Add(new UnitTestTask(this, behaviorSpecificationTask));

            return sequence;
        }

        public bool Equals(BehaviorSpecificationElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(FieldName, other.FieldName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BehaviorSpecificationElement);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(Id)
                .And(TypeName?.FullName)
                .And(FieldName);
        }
    }
}
