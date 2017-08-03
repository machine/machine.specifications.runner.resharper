using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.ReSharperProvider.Presentation;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    [SolutionComponent]
    public class ContextSpecificationFactory
    {
        private readonly ElementCache _cache;
        private readonly UnitTestingCachingService _cachingService;
        private readonly IUnitTestElementManager _manager;
        private readonly IUnitTestElementIdFactory _elementIdFactory;
        private readonly MSpecUnitTestProvider _provider;
        private readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

        public ContextSpecificationFactory(MSpecUnitTestProvider provider,
                                           IUnitTestElementManager manager,
                                           IUnitTestElementIdFactory elementIdFactory,
                                           UnitTestingCachingService cachingService,
                                           ElementCache cache)
        {
            _manager = manager;
            _elementIdFactory = elementIdFactory;
            _cachingService = cachingService;
            _provider = provider;
            _cache = cache;
        }

        public ContextSpecificationElement CreateContextSpecification(IUnitTestElementsObserver consumer, IDeclaredElement field)
        {
            var contextClass = ((ITypeMember)field).GetContainingType() as IClass;

            if (contextClass == null)
                return null;

            var context = _cache.TryGetContext(contextClass);

            if (context == null)
                return null;

            return GetOrCreateContextSpecification(consumer, context, contextClass.GetClrName(), field.ShortName, field.IsIgnored());
        }

        public ContextSpecificationElement CreateContextSpecification(IUnitTestElementsObserver consumer, ContextElement context, IMetadataField specification)
        {
            return GetOrCreateContextSpecification(consumer, context,
                _reflectionTypeNameCache.GetClrName(specification.DeclaringType), specification.Name,
                specification.IsIgnored());
        }

        private ContextSpecificationElement GetOrCreateContextSpecification(IUnitTestElementsObserver consumer,
                                                                           ContextElement context,
                                                                           IClrTypeName declaringTypeName,
                                                                           string fieldName,
                                                                           bool isIgnored)
        {
            var id = ContextSpecificationElement.CreateId(_elementIdFactory, consumer, _provider, context, fieldName);

            var contextSpecification = _manager.GetElementById(id) as ContextSpecificationElement;

            if (contextSpecification != null)
            {
                contextSpecification.Parent = context;
                return contextSpecification;
            }

            return new ContextSpecificationElement(_provider, id, context, declaringTypeName.GetPersistent(),
                _cachingService, _manager, fieldName, isIgnored);
        }
    }
}