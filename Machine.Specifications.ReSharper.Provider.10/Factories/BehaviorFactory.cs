using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.ReSharperProvider.Presentation;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    [SolutionComponent]
    public class BehaviorFactory
    {
        private readonly ElementCache _cache;
        private readonly IUnitTestElementManager _manager;
        private readonly IUnitTestElementIdFactory _elementIdFactory;
        private readonly UnitTestingCachingService _cachingService;
        private readonly MSpecUnitTestProvider _provider;

        public BehaviorFactory(MSpecUnitTestProvider provider,
                             IUnitTestElementManager manager,
                             IUnitTestElementIdFactory elementIdFactory,
                             UnitTestingCachingService cachingService,
                             ElementCache cache)
        {
            _provider = provider;
            _cache = cache;
            _manager = manager;
            _elementIdFactory = elementIdFactory;
            _cachingService = cachingService;
        }

        public BehaviorElement CreateBehavior(IDeclaredElement field, IUnitTestElementsObserver consumer)
        {
            var contextClass = ((ITypeMember)field).GetContainingType() as IClass;

            if (contextClass == null)
                return null;

            var context = _cache.TryGetContext(contextClass);

            if (context == null)
                return null;

            var fieldType = new NormalizedTypeName(field as ITypeOwner);

            return GetOrCreateBehavior(context, consumer, contextClass.GetClrName(), field.ShortName, field.IsIgnored(), fieldType);
        }

        public BehaviorElement CreateBehavior(ContextElement context, IMetadataField behavior, IUnitTestElementsObserver consumer)
        {
            var typeContainingBehaviorSpecifications = behavior.GetFirstGenericArgument();

            var metadataTypeName = behavior.FirstGenericArgumentClass().FullyQualifiedName();
            var fieldType = new NormalizedTypeName(metadataTypeName);

            return GetOrCreateBehavior(context, consumer, context.GetTypeClrName(), behavior.Name,
                behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(), fieldType);
        }

        private BehaviorElement GetOrCreateBehavior(ContextElement context,
                                                   IUnitTestElementsObserver consumer,
                                                   IClrTypeName declaringTypeName,
                                                   string fieldName,
                                                   bool isIgnored,
                                                   string fieldType)
        {
            var id = BehaviorElement.CreateId(_elementIdFactory, consumer, _provider, context, fieldType, fieldName);
            var behavior = _manager.GetElementById(id) as BehaviorElement;

            if (behavior != null)
            {
                behavior.Parent = context;
                return behavior;
            }

            return new BehaviorElement(id, context, declaringTypeName.GetPersistent(), _cachingService,
                _manager, fieldName, isIgnored, fieldType);
        }
    }
}