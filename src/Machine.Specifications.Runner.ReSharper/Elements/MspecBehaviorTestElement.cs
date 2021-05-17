using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecBehaviorTestElement : FieldElement, IEquatable<MspecBehaviorTestElement>
    {
        public MspecBehaviorTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string fieldName, string explicitReason)
            : base(services, id, typeName, fieldName, explicitReason)
        {
        }

        public MspecContextTestElement Context => Parent as MspecContextTestElement;

        public override string Kind => "Behavior";

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var sequence = Context.GetTaskSequence(explicitElements, run);

            var behaviorTask = run.GetRemoteTaskForElement<MspecBehaviorRunnerTask>(this) ??
                               new MspecBehaviorRunnerTask(Id.ProjectId, Context.TypeName.FullName, FieldName, ExplicitReason);

            sequence.Add(new UnitTestTask(this, behaviorTask));

            return sequence;
        }

        protected override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public bool Equals(MspecBehaviorTestElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(FieldName, other.FieldName);
        }
    }
}
