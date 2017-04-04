using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ContextElement : Element
    {
        readonly ISet<UnitTestElementCategory> _categories;
        readonly UnitTestElementId _id;
        readonly string _subject;

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
            this._id = id;
            this.AssemblyLocation = assemblyLocation;
            this._subject = subject;

            if (tags != null)
            {
                this._categories = categoryFactory.Create(tags);
            }
        }

        public override string ShortName
        {
            get { return this.Kind + this.GetPresentation(); }
        }

        public string AssemblyLocation { get; set; }

        public override string Kind
        {
            get { return "Context"; }
        }

        public override ISet<UnitTestElementCategory> OwnCategories
        {
            get { return this._categories; }
        }

        public override UnitTestElementId Id
        {
            get { return this._id; }
        }


        public override string GetPresentation()
        {
            return this.GetSubject() + this.GetTypeClrName().ShortName.ToFormat();
        }

        string GetSubject()
        {
            if (String.IsNullOrEmpty(this._subject))
            {
                return null;
            }

            return this._subject + ", ";
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return this.GetDeclaredType();
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, IUnitTestProvider provider, IProject project, string subject, string typeName, IEnumerable<string> tags)
        {
            string tagsAsString = null;
            if (tags != null)
            {
                tagsAsString = tags.AggregateString("", "|", (builder, tag) => builder.Append(tag));
            }
            var result = new[] { subject, typeName, tagsAsString };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return elementIdFactory.Create(provider, project, consumer.TargetFrameworkId, id);
        }
    }
}