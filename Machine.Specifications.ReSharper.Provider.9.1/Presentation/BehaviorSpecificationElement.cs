namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;
    using Machine.Specifications.ReSharperProvider.Shims;

    public class BehaviorSpecificationElement : FieldElement
    {
        readonly UnitTestElementId _id;

        public BehaviorSpecificationElement(MSpecUnitTestProvider provider,
                                            IPsi psiModuleManager,
                                            ICache cacheManager,
                                            ProjectModelElementEnvoy projectEnvoy,
                                            BehaviorElement behavior,
                                            IClrTypeName declaringTypeName,
                                            string fieldName,
                                            bool isIgnored
          )
            : base(provider,
                   psiModuleManager,
                   cacheManager,
                   behavior,
                   projectEnvoy,
                   declaringTypeName,
                   fieldName,
                   isIgnored || behavior.Explicit)
        {
            this._id = CreateId(provider, behavior, fieldName);
        }

        public BehaviorElement Behavior
        {
            get { return (BehaviorElement)this.Parent; }
        }

        public override string Kind
        {
            get { return "Behavior Specification"; }
        }

        public override IEnumerable<UnitTestElementCategory> Categories
        {
            get
            {
                var parent = this.Parent ?? this.Behavior;
                if (parent == null)
                {
                    return UnitTestElementCategory.Uncategorized;
                }

                return parent.Categories;
            }
        }

        public override UnitTestElementId Id
        {
            get { return this._id; }
        }

        public static UnitTestElementId CreateId(IUnitTestProvider provider, BehaviorElement behaviorElement, string fieldName)
        {
            var result = new[] { behaviorElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return new UnitTestElementId(provider, new PersistentProjectId(behaviorElement.GetProject()), id);
        }
    }
}
