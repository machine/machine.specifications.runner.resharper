using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Runner;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class ContextElement : Element, IEquatable<ContextElement>
    {
        public ContextElement(
            UnitTestElementId id,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string subject,
            bool isIgnored)
            : base(id, null, typeName, serviceProvider, isIgnored)
        {
            Subject = subject;
        }

        public override string ShortName => Kind + GetPresentation();

        public override string Kind => "Context";

        public string Subject { get; }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var task = run.GetRemoteTaskForElement<MspecContextTask>(this) ??
                       new MspecContextTask(Id.ProjectId, TypeName.FullName);

            return new List<UnitTestTask>
            {
                new(null, new MspecBootstrapTask(Id.ProjectId)),
                new(null, new MspecAssemblyTask(Id.ProjectId, Id.Project.GetOutputFilePath(Id.TargetFrameworkId).FullPath)),
                new(this, task)
            };
        }

        protected override string GetPresentation()
        {
            var display = TypeName.ShortName.ToFormat();

            return string.IsNullOrEmpty(Subject)
                ? display
                : $"{Subject}, {display}";
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return GetDeclaredType();
        }

        public bool Equals(ContextElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContextElement);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(Id)
                .And(TypeName);
        }
    }
}
