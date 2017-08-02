using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.ReSharperProvider.Presentation;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    [SolutionComponent]
    public class ContextFactory
    {
        private readonly ElementCache _cache;
        private readonly IUnitTestElementManager _manager;
        private readonly IUnitTestElementIdFactory _elementIdFactory;
        private readonly IUnitTestElementCategoryFactory _categoryFactory;
        private readonly UnitTestingCachingService _cachingService;
        private readonly MSpecUnitTestProvider _provider;

        public ContextFactory(MSpecUnitTestProvider provider,
                              IUnitTestElementManager manager,
                              IUnitTestElementIdFactory elementIdFactory,
                              IUnitTestElementCategoryFactory categoryFactory,
                              UnitTestingCachingService cachingService,
                              ElementCache cache)
        {
            _manager = manager;
            _elementIdFactory = elementIdFactory;
            _categoryFactory = categoryFactory;
            _cachingService = cachingService;
            _provider = provider;
            _cache = cache;
        }

        public IUnitTestElement CreateContext(IUnitTestElementsObserver consumer, string assemblyPath, IDeclaration contextDeclaration)
        {
            var contextType = (ITypeElement)contextDeclaration.DeclaredElement;
            var context = GetOrCreateContext(consumer, assemblyPath, contextDeclaration.GetProject(),
                contextType.GetClrName(), contextType.GetSubjectString(), contextType.GetTags(),
                contextType.IsIgnored());

            _cache.AddContext(contextType, context);

            return context;
        }

        public ContextElement CreateContext(IUnitTestElementsObserver consumer, IProject project, string assemblyPath, IMetadataTypeInfo contextType)
        {
            return GetOrCreateContext(consumer, assemblyPath, project, new ClrTypeName(contextType.FullyQualifiedName),
                contextType.GetSubjectString(), contextType.GetTags(), contextType.IsIgnored());
        }

        public ContextElement GetOrCreateContext(IUnitTestElementsObserver consumer,
                                                 string assemblyPath,
                                                 IProject project,
                                                 IClrTypeName contextTypeName,
                                                 string subject,
                                                 ICollection<string> tags,
                                                 bool isIgnored)
        {
            UnitTestElementId id = ContextElement.CreateId(_elementIdFactory, consumer, _provider, project, subject, contextTypeName.FullName, tags);

            var contextElement = _manager.GetElementById(id) as ContextElement;

            if (contextElement != null)
            {
                contextElement.AssemblyLocation = assemblyPath;
                return contextElement;
            }

            return new ContextElement(_provider, id, contextTypeName.GetPersistent(), _cachingService, _manager,
                assemblyPath, subject, tags, isIgnored, _categoryFactory);
        }
    }
}