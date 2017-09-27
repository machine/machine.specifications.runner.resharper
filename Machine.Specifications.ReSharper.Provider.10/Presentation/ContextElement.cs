using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.Presentation
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
            var display = TypeName.ShortName.ToLower();

            if (string.IsNullOrEmpty(_subject))
                return display;

            return $"{_subject}, {display}";
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return GetDeclaredType();
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
            var result = Id != null ? Id.GetHashCode() : 0;
            result = (result * 397) ^ (TypeName != null ? TypeName.FullName.GetHashCode() : 0);
            result = (result * 397) ^ (AssemblyLocation != null ? AssemblyLocation.GetHashCode() : 0);

            return result;
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