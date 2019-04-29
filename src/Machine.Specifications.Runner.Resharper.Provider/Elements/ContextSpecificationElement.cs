using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.ReSharperRunner;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperProvider.Elements
{
    public class ContextSpecificationElement : FieldElement, IEquatable<ContextSpecificationElement>
    {
        public ContextSpecificationElement(
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

        public override string Kind => "Specification";

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var contextTask = run.GetRemoteTaskForElement<MspecTestContextTask>(Context) ??
                              new MspecTestContextTask(Id.ProjectId, Context.TypeName.FullName);

            var task = run.GetRemoteTaskForElement<MspecTestSpecificationTask>(this) ??
                       new MspecTestSpecificationTask(Id.ProjectId, Context.TypeName.FullName, FieldName);

            return new List<UnitTestTask>
            {
                new UnitTestTask(null, new MspecTestAssemblyTask(Id.ProjectId, Context.AssemblyLocation.FullPath)),
                new UnitTestTask(Context, contextTask),
                new UnitTestTask(this, task)
            };
        }

        public bool Equals(ContextSpecificationElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName.FullName, other.TypeName.FullName) &&
                   Equals(FieldName, other.FieldName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContextSpecificationElement);
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
