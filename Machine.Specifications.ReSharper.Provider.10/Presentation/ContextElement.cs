using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.Util;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class ContextElement : Element
    {
        private readonly string _subject;

        public ContextElement(MSpecUnitTestProvider provider,
                              UnitTestElementId id,
                              IClrTypeName typeName,
                              UnitTestingCachingService cachingService,
                              IUnitTestElementManager elementManager,
                              string assemblyLocation,
                              string subject,
                              IEnumerable<string> tags,
                              bool isIgnored,
                              IUnitTestElementCategoryFactory categoryFactory)
            : base(provider, null, typeName, cachingService, elementManager, isIgnored)
        {
            Id = id;
            AssemblyLocation = assemblyLocation;
            _subject = subject;

            if (tags != null)
                OwnCategories = categoryFactory.Create(tags);
        }

        public override string ShortName => Kind + GetPresentation();

        public string AssemblyLocation { get; set; }

        public override string Kind => "Context";

        public override ISet<UnitTestElementCategory> OwnCategories { get; }

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