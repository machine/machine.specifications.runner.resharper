using JetBrains.Metadata.Reader.API;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;
    using Machine.Specifications.ReSharperProvider.Shims;

    public class ContextElement : Element
    {
        readonly IEnumerable<UnitTestElementCategory> _categories;
        readonly UnitTestElementId _id;
        readonly string _subject;

        public ContextElement(MSpecUnitTestProvider provider,
                              IPsi psiModuleManager,
                              ICache cacheManager,
                              UnitTestElementId id,
                              ProjectModelElementEnvoy projectEnvoy,
                              IClrTypeName typeName,
                              string assemblyLocation,
                              string subject,
                              IEnumerable<string> tags,
                              bool isIgnored,
                              IUnitTestCategoryFactory categoryFactory)
            : base(provider, psiModuleManager, cacheManager, null, projectEnvoy, typeName, isIgnored)
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

        public override IEnumerable<UnitTestElementCategory> Categories
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

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestProvider provider, IProject project, string subject, string typeName, IEnumerable<string> tags)
        {
            string tagsAsString = null;
            if (tags != null)
            {
                tagsAsString = tags.AggregateString("", "|", (builder, tag) => builder.Append(tag));
            }
            var result = new[] { subject, typeName, tagsAsString };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return elementIdFactory.Create(provider, new PersistentProjectId(project), id);
        }
    }
}