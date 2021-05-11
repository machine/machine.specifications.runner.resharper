using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Runner;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class BehaviorElement : FieldElement, IEquatable<BehaviorElement>
    {
        public BehaviorElement(
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored)
            : base(id, parent, typeName, serviceProvider, fieldName, isIgnored || parent.Explicit)
        {
        }

        public ContextElement Context => Parent as ContextElement;

        public override string Kind => "Behavior";

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var sequence = Context.GetTaskSequence(explicitElements, run);

            var behaviorTask = run.GetRemoteTaskForElement<MspecBehaviorRunnerTask>(this) ??
                               new MspecBehaviorRunnerTask(Id.ProjectId, Context.TypeName.FullName, FieldName);

            sequence.Add(new UnitTestTask(this, behaviorTask));

            return sequence;
        }

        protected override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public bool Equals(BehaviorElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(FieldName, other.FieldName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BehaviorElement);
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
