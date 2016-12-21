namespace Machine.Specifications.ReSharperProvider.Factories
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Impl.Reflection2;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Elements;

    using Machine.Specifications.ReSharperProvider.Presentation;

    [SolutionComponent]
    public class ContextSpecificationFactory
    {
        readonly ElementCache _cache;
        private readonly UnitTestingCachingService _cachingService;
        readonly IUnitTestElementManager _manager;
        readonly IUnitTestElementIdFactory _elementIdFactory;
        readonly MSpecUnitTestProvider _provider;
        readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

        public ContextSpecificationFactory(MSpecUnitTestProvider provider,
                                           IUnitTestElementManager manager,
                                           IUnitTestElementIdFactory elementIdFactory,
                                           UnitTestingCachingService cachingService,
                                           ElementCache cache)
        {
            this._manager = manager;
            this._elementIdFactory = elementIdFactory;
            this._cachingService = cachingService;
            this._provider = provider;
            this._cache = cache;
        }

        public ContextSpecificationElement CreateContextSpecification(IUnitTestElementsObserver consumer, IDeclaredElement field)
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

            return this.GetOrCreateContextSpecification(consumer, 
                                                   context,
                                                   contextClass.GetClrName(),
                                                   field.ShortName,
                                                   field.IsIgnored());
        }

        public ContextSpecificationElement CreateContextSpecification(IUnitTestElementsObserver consumer, ContextElement context, IMetadataField specification)
        {
            return this.GetOrCreateContextSpecification(consumer, 
                                                   context,
                                                   this._reflectionTypeNameCache.GetClrName(specification.DeclaringType),
                                                   specification.Name,
                                                   specification.IsIgnored());
        }

        public ContextSpecificationElement GetOrCreateContextSpecification(IUnitTestElementsObserver consumer,
                                                                           ContextElement context,
                                                                           IClrTypeName declaringTypeName,
                                                                           string fieldName,
                                                                           bool isIgnored)
        {
            var id = ContextSpecificationElement.CreateId(_elementIdFactory, consumer, _provider, context, fieldName);

            var contextSpecification = this._manager.GetElementById(id) as ContextSpecificationElement;
            if (contextSpecification != null)
            {
                contextSpecification.Parent = context;
                return contextSpecification;
            }

            return new ContextSpecificationElement(this._provider,
                                                   id,
                                                   context,
                                                   declaringTypeName.GetPersistent(),
                                                   this._cachingService,
                                                   this._manager,
                                                   fieldName,
                                                   isIgnored);
        }
    }
}