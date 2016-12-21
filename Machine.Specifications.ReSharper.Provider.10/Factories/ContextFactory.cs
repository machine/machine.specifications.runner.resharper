using JetBrains.Metadata.Reader.Impl;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Elements;
    using Machine.Specifications.ReSharperProvider.Presentation;
    using System.Collections.Generic;

    [SolutionComponent]
    public class ContextFactory
    {
        readonly ElementCache _cache;
        readonly IUnitTestElementManager _manager;
        readonly IUnitTestElementIdFactory _elementIdFactory;
        readonly IUnitTestElementCategoryFactory _categoryFactory;
        private readonly UnitTestingCachingService _cachingService;
        readonly MSpecUnitTestProvider _provider;

        public ContextFactory(MSpecUnitTestProvider provider,
                              IUnitTestElementManager manager,
                              IUnitTestElementIdFactory elementIdFactory,
                              IUnitTestElementCategoryFactory categoryFactory,
                              UnitTestingCachingService cachingService,
                              ElementCache cache)
        {
            this._manager = manager;
            this._elementIdFactory = elementIdFactory;
            this._categoryFactory = categoryFactory;
            _cachingService = cachingService;
            this._provider = provider;
            this._cache = cache;
        }

        public IUnitTestElement CreateContext(IUnitTestElementsObserver consumer, string assemblyPath, IDeclaration contextDeclaration)
        {
            var contextType = (ITypeElement)contextDeclaration.DeclaredElement;
            var context = this.GetOrCreateContext(consumer, 
                                           assemblyPath,
                                           contextDeclaration.GetProject(),
                                           contextType.GetClrName(),
                                           contextType.GetSubjectString(),
                                           contextType.GetTags(),
                                           contextType.IsIgnored());

            this._cache.AddContext(contextType, context);
            return context;
        }

        public ContextElement CreateContext(IUnitTestElementsObserver consumer, IProject project, string assemblyPath, IMetadataTypeInfo contextType)
        {
            return this.GetOrCreateContext(consumer, 
                                      assemblyPath,
                                      project,
                                      new ClrTypeName(contextType.FullyQualifiedName),
                                      contextType.GetSubjectString(),
                                      contextType.GetTags(),
                                      contextType.IsIgnored());
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

            var contextElement = this._manager.GetElementById(id) as ContextElement;
            if (contextElement != null)
            {
                contextElement.AssemblyLocation = assemblyPath;
                return contextElement;
            }

            return new ContextElement(this._provider,
                                      id,
                                      contextTypeName.GetPersistent(),
                                      this._cachingService,
                                      this._manager,
                                      assemblyPath,
                                      subject,
                                      tags,
                                      isIgnored,
                                      _categoryFactory);
        }
    }
}