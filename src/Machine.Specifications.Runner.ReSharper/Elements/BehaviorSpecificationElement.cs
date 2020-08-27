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
            var context = Behavior.Context;
            var contextName = context.TypeName.FullName;

            var contextTask = run.GetRemoteTaskForElement<MspecContextTask>(Behavior.Context) ??
                              new MspecContextTask(Id.ProjectId, contextName);

            var behaviorTask = run.GetRemoteTaskForElement<MspecBehaviorTask>(Behavior) ??
                               new MspecBehaviorTask(Id.ProjectId, contextName, Behavior.FieldName);

            var behaviorSpecificationTask = run.GetRemoteTaskForElement<MspecBehaviorSpecificationTask>(this) ??
                                            new MspecBehaviorSpecificationTask(Id.ProjectId, contextName, Behavior.FieldName, FieldName);

            return new List<UnitTestTask>
            {
                new UnitTestTask(null, new MspecAssemblyTask(Id.ProjectId, context.Id.Project.GetOutputFilePath(Id.TargetFrameworkId).FullPath)),
                new UnitTestTask(context, contextTask),
                new UnitTestTask(Behavior, behaviorTask),
                new UnitTestTask(this, behaviorSpecificationTask)
            };
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
