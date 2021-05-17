using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecContextSpecificationTestElement : FieldElement, IEquatable<MspecContextSpecificationTestElement>
    {
        public MspecContextSpecificationTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string fieldName, string explicitReason)
            : base(services, id, typeName, fieldName, explicitReason)
        {
        }

        public MspecContextTestElement Context => Parent as MspecContextTestElement;

        public override string Kind => "Specification";

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var sequence = Context.GetTaskSequence(explicitElements, run);

            var task = run.GetRemoteTaskForElement<MspecContextSpecificationRunnerTask>(this) ??
                       new MspecContextSpecificationRunnerTask(Id.ProjectId, Context.TypeName.FullName, FieldName);

            sequence.Add(new UnitTestTask(this, task));

            return sequence;
        }

        public bool Equals(MspecContextSpecificationTestElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName.FullName, other.TypeName.FullName) &&
                   Equals(FieldName, other.FieldName);
        }
    }
}
