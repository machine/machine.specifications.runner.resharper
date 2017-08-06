using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.ReSharperProvider.Presentation;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    [SolutionComponent]
    public class BehaviorSpecificationFactory
    {
        private readonly IUnitTestElementIdFactory _elementIdFactory;
        private readonly MSpecUnitTestProvider _provider;
        private readonly IUnitTestElementManager _manager;
        private readonly UnitTestingCachingService _cachingService;

        public BehaviorSpecificationFactory(MSpecUnitTestProvider provider,
                                            IUnitTestElementManager manager,
                                            UnitTestingCachingService cachingService,
                                            IUnitTestElementIdFactory elementIdFactory)
        {
            _elementIdFactory = elementIdFactory;
            _provider = provider;
            _manager = manager;
            _cachingService = cachingService;
        }

        public IEnumerable<BehaviorSpecificationElement> CreateBehaviorSpecificationsFromBehavior(
            BehaviorElement behavior,
            IMetadataField behaviorSpecification,
            IUnitTestElementsObserver consumer)
        {
            var type = behaviorSpecification.GetFirstGenericArgument();

            foreach (var specification in type.GetSpecifications())
                yield return CreateBehaviorSpecification(behavior, specification, consumer);
        }

        internal BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                                          IDeclaredElement behaviorSpecification,
                                                                          IUnitTestElementsObserver consumer)
        {
            return GetOrCreateBehaviorSpecification(behavior, consumer,
                ((ITypeMember) behaviorSpecification).GetContainingType().GetClrName(), behaviorSpecification.ShortName,
                behaviorSpecification.IsIgnored());
        }

        BehaviorSpecificationElement CreateBehaviorSpecification(BehaviorElement behavior,
                                                                 IMetadataField behaviorSpecification,
                                                                 IUnitTestElementsObserver consumer)
        {
            return GetOrCreateBehaviorSpecification(behavior, consumer,
                new ClrTypeName(behaviorSpecification.DeclaringType.FullyQualifiedName), behaviorSpecification.Name,
                behaviorSpecification.IsIgnored());
        }

        private BehaviorSpecificationElement GetOrCreateBehaviorSpecification(BehaviorElement behavior,
                                                                             IUnitTestElementsObserver consumer,
                                                                             IClrTypeName declaringTypeName,
                                                                             string fieldName,
                                                                             bool isIgnored)
        {
            var id = BehaviorSpecificationElement.CreateId(_elementIdFactory, consumer, _provider, behavior, fieldName);

            var behaviorSpecification = _manager.GetElementById(id) as BehaviorSpecificationElement;

            if (behaviorSpecification != null)
            {
                behaviorSpecification.Parent = behavior;
                return behaviorSpecification;
            }

            return new BehaviorSpecificationElement(id, behavior, declaringTypeName.GetPersistent(),
                _cachingService, _manager, fieldName, isIgnored);
        }
    }
}