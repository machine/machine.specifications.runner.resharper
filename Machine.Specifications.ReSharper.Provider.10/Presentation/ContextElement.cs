using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class ContextElement : Element
    {
        private readonly string _subject;

        public ContextElement(
            UnitTestElementId id,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string subject,
            bool isIgnored)
            : base(null, typeName, serviceProvider, isIgnored)
        {
            Id = id;
            _subject = subject;
        }

        public override string ShortName => Kind + GetPresentation();

        public FileSystemPath AssemblyLocation { get; set; }

        public override string Kind => "Context";

        public override UnitTestElementId Id { get; }

        protected override string GetPresentation()
        {
            return GetSubject() + GetTypeClrName().ShortName.ToFormat();
        }

        private string GetSubject()
        {
            if (string.IsNullOrEmpty(_subject))
                return null;

            return _subject + ", ";
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return GetDeclaredType();
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, IUnitTestProvider provider, IProject project, string subject, string typeName, IEnumerable<string> tags)
        {
            string tagsAsString = null;

            if (tags != null)
                tagsAsString = tags.AggregateString("", "|", (builder, tag) => builder.Append(tag));

            var result = new[] { subject, typeName, tagsAsString };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, project, consumer.TargetFrameworkId, id);
        }
    }
}