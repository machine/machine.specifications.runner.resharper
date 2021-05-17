using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecBehaviorSpecificationTestElement : MspecFieldTestElement, IEquatable<MspecBehaviorSpecificationTestElement>
    {
        public MspecBehaviorSpecificationTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string fieldName, string explicitReason)
            : base(services, id, typeName, fieldName, explicitReason)
        {
        }

        public MspecBehaviorTestElement Behavior => Parent as MspecBehaviorTestElement;

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
                                            new MspecBehaviorSpecificationRunnerTask(Id.ProjectId, Behavior.Context.TypeName.FullName, Behavior.FieldName, FieldName, ExplicitReason);

            sequence.Add(new UnitTestTask(this, behaviorSpecificationTask));

            return sequence;
        }

        public bool Equals(MspecBehaviorSpecificationTestElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(FieldName, other.FieldName);
        }
    }
}
