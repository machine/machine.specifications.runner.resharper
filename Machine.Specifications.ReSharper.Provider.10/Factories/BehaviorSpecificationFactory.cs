using JetBrains.Metadata.Reader.Impl;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    using System.Collections.Generic;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Elements;
    using Machine.Specifications.ReSharperProvider.Presentation;

    [SolutionComponent]
    public class BehaviorSpecificationFactory
    {
        readonly IUnitTestElementIdFactory _elementIdFactory;
        readonly MSpecUnitTestProvider _provider;
        private readonly IUnitTestElementManager _manager;
        private readonly UnitTestingCachingService _cachingService;

        public BehaviorSpecificationFactory(MSpecUnitTestProvider provider,
                                            IUnitTestElementManager manager,
                                            UnitTestingCachingService cachingService,
                                            IUnitTestElementIdFactory elementIdFactory)
        {
            this._elementIdFactory = elementIdFactory;
            this._provider = provider;
            _manager = manager;
            _cachingService = cachingService;
        }

        public IEnumerable<BehaviorSpecificationElement> CreateBehaviorSpecificationsFromBehavior(
          BehaviorElement behavior,
          IMetadataField behaviorSpecification)
        {
            var typeContainingBehaviorSpecifications = behaviorSpecification.GetFirstGenericArgument();

            foreach (var specification in typeContainingBehaviorSpecifications.GetSpecifications())
            {
                yield return this.CreateBehaviorSpecification(behavior, specification);
            }
        }

        internal BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                                          IDeclaredElement behaviorSpecification)
        {
            return this.GetOrCreateBehaviorSpecification(behavior,
                                                    ((ITypeMember)behaviorSpecification).GetContainingType().GetClrName(),
                                                    behaviorSpecification.ShortName,
                                                    behaviorSpecification.IsIgnored());
        }

        BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                                 IMetadataField behaviorSpecification)
        {
            return this.GetOrCreateBehaviorSpecification(behavior,
                                                    new ClrTypeName(behaviorSpecification.DeclaringType.FullyQualifiedName),
                                                    behaviorSpecification.Name,
                                                    behaviorSpecification.IsIgnored());
        }

        public BehaviorSpecificationElement GetOrCreateBehaviorSpecification(BehaviorElement behavior,
                                                                             IClrTypeName declaringTypeName,
                                                                             string fieldName,
                                                                             bool isIgnored)
        {
            var id = BehaviorSpecificationElement.CreateId(_elementIdFactory, _provider, behavior, fieldName);
            var behaviorSpecification = this._manager.GetElementById(id) as BehaviorSpecificationElement;
            if (behaviorSpecification != null)
            {
                behaviorSpecification.Parent = behavior;
                return behaviorSpecification;
            }

            return new BehaviorSpecificationElement(this._provider,
                                                    id,
                                                    behavior,
                                                    declaringTypeName.GetPersistent(),
                                                    this._cachingService,
                                                    fieldName,
                                                    isIgnored);
        }
    }
}