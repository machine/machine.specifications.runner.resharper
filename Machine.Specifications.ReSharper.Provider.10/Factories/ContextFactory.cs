using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.Util;
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
        private readonly MspecServiceProvider _serviceProvider;
        private readonly MspecTestProvider _provider;

        public ContextFactory(MspecTestProvider provider,
                              IUnitTestElementManager manager,
                              IUnitTestElementIdFactory elementIdFactory,
                              IUnitTestElementCategoryFactory categoryFactory,
                              MspecServiceProvider serviceProvider,
                              ElementCache cache)
        {
            _manager = manager;
            _elementIdFactory = elementIdFactory;
            _categoryFactory = categoryFactory;
            _serviceProvider = serviceProvider;
            _provider = provider;
            _cache = cache;
        }

        public IUnitTestElement CreateContext(IUnitTestElementsObserver consumer, FileSystemPath assemblyPath, IDeclaration contextDeclaration)
        {
            var contextType = (ITypeElement)contextDeclaration.DeclaredElement;
            var context = GetOrCreateContext(consumer, assemblyPath, contextDeclaration.GetProject(),
                contextType.GetClrName(), contextType.GetSubjectString(), contextType.GetTags(),
                contextType.IsIgnored());

            _cache.AddContext(contextType, context);

            return context;
        }

        public ContextElement CreateContext(IUnitTestElementsObserver consumer, IProject project, FileSystemPath assemblyPath, IMetadataTypeInfo contextType)
        {
            return GetOrCreateContext(consumer, assemblyPath, project, new ClrTypeName(contextType.FullyQualifiedName),
                contextType.GetSubjectString(), contextType.GetTags(), contextType.IsIgnored());
        }

        public ContextElement GetOrCreateContext(IUnitTestElementsObserver consumer,
                                                 FileSystemPath assemblyPath,
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

            var element = new ContextElement(id, contextTypeName.GetPersistent(), _serviceProvider,
                subject, isIgnored);

            element.AssemblyLocation = assemblyPath;
            element.OwnCategories = _categoryFactory.Create(tags);

            return element;
        }
    }
}