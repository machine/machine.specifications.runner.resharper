using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Elements;
    using Machine.Specifications.ReSharperProvider.Presentation;

    [SolutionComponent]
    public class BehaviorFactory
    {
        readonly ElementCache _cache;
        readonly IUnitTestElementManager _manager;
        readonly IUnitTestElementIdFactory _elementIdFactory;
        private readonly UnitTestingCachingService _cachingService;
        readonly MSpecUnitTestProvider _provider;

        public BehaviorFactory(MSpecUnitTestProvider provider,
                             IUnitTestElementManager manager,
                             IUnitTestElementIdFactory elementIdFactory,
                             UnitTestingCachingService cachingService,
                             ElementCache cache)
        {
            this._provider = provider;
            this._cache = cache;
            this._manager = manager;
            this._elementIdFactory = elementIdFactory;
            this._cachingService = cachingService;
        }

        public BehaviorElement CreateBehavior(IDeclaredElement field)
        {
            var contextClass = ((ITypeMember)field).GetContainingType() as IClass;
            if (contextClass == null)
            {
                return null;
            }

            var context = this._cache.TryGetContext(contextClass);
            if (context == null)
            {
                return null;
            }

            var fieldType = new NormalizedTypeName(field as ITypeOwner);

            var behavior = this.GetOrCreateBehavior(context,
                                               contextClass.GetClrName(),
                                               field.ShortName,
                                               field.IsIgnored(),
                                               fieldType);

            this._cache.AddBehavior(field, behavior);
            return behavior;
        }

        public BehaviorElement CreateBehavior(ContextElement context, IMetadataField behavior)
        {
            var typeContainingBehaviorSpecifications = behavior.GetFirstGenericArgument();

            var metadataTypeName = behavior.FirstGenericArgumentClass().FullyQualifiedName();
            var fieldType = new NormalizedTypeName(metadataTypeName);

            var behaviorElement = this.GetOrCreateBehavior(context,
                                                      context.GetTypeClrName(),
                                                      behavior.Name,
                                                      behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(),
                                                      fieldType);

            return behaviorElement;
        }

        public BehaviorElement GetOrCreateBehavior(ContextElement context,
                                                   IClrTypeName declaringTypeName,
                                                   string fieldName,
                                                   bool isIgnored,
                                                   string fieldType)
        {
            var id = BehaviorElement.CreateId(_elementIdFactory, _provider, context, fieldType, fieldName);
            var behavior = this._manager.GetElementById(id) as BehaviorElement;
            if (behavior != null)
            {
                behavior.Parent = context;
                return behavior;
            }

            return new BehaviorElement(this._provider,
                                       id,
                                       context,
                                       declaringTypeName.GetPersistent(),
                                       this._cachingService,
                                       fieldName,
                                       isIgnored,
                                       fieldType);
        }
    }
}