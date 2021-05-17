using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecContextTestElement : MspecTestElement, IEquatable<MspecContextTestElement>
    {
        public MspecContextTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string subject, string explicitReason)
            : base(services, id, typeName, explicitReason)
        {
            Subject = subject;
            ShortName = typeName.ShortName.ToFormat();
        }

        public override string Kind => "Context";

        public string Subject { get; }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var task = run.GetRemoteTaskForElement<MspecContextRunnerTask>(this) ??
                       new MspecContextRunnerTask(Id.ProjectId, TypeName.FullName, ExplicitReason);

            return new List<UnitTestTask>
            {
                new(null, new MspecBootstrapRunnerTask(Id.ProjectId)),
                new(null, new MspecAssemblyRunnerTask(Id.ProjectId, Id.Project.GetOutputFilePath(Id.TargetFrameworkId).FullPath)),
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

        public bool Equals(MspecContextTestElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName);
        }
    }
}
