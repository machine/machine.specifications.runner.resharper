using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.Util;
using Machine.Specifications.ReSharperRunner;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider.Elements
{
    public class ContextElement : Element, IEquatable<ContextElement>
    {
        private readonly string _subject;

        public ContextElement(
            UnitTestElementId id,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string subject,
            bool isIgnored)
            : base(id, null, typeName, serviceProvider, isIgnored)
        {
            _subject = subject;
        }

        public override string ShortName => Kind + GetPresentation();

        public FileSystemPath AssemblyLocation { get; set; }

        public override string Kind => "Context";

        protected override string GetPresentation()
        {
            var display = TypeName.ShortName.ToFormat();

            if (string.IsNullOrEmpty(_subject))
                return display;

            return $"{_subject}, {display}";
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return GetDeclaredType();
        }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            var task = run.GetRemoteTaskForElement(this) ??
                       new MspecTestContextTask(Id.ProjectId, TypeName.FullName);

            return new List<UnitTestTask>
            {
                new UnitTestTask(null, new MspecTestAssemblyTask(Id.ProjectId, AssemblyLocation.FullPath)),
                new UnitTestTask(this, task)
            };
        }

        public bool Equals(ContextElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(AssemblyLocation, other.AssemblyLocation);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContextElement);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(Id)
                .And(TypeName)
                .And(AssemblyLocation);
        }
    }
}
