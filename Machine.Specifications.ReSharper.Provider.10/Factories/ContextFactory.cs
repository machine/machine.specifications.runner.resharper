using JetBrains.Metadata.Reader.Impl;
using Machine.Specifications.ReSharperRunner;

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
    using System.Linq;

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

        public IUnitTestElement CreateContext(string assemblyPath, IDeclaration declaration)
        {
            var type = (ITypeElement)declaration.DeclaredElement;
            var context = this.GetOrCreateContext(assemblyPath,
                                             declaration.GetProject(),
                                             type.GetClrName().GetPersistent(),
                                             type.GetSubjectString(),
                                             type.GetTags(), type.IsIgnored());

            foreach (var child in context.Children)
            {
                child.State = UnitTestElementState.Pending;
            }

            this._cache.AddContext(type, context);
            return context;
        }

        public ContextElement CreateContext(IProject project, string assemblyPath, IMetadataTypeInfo type)
        {
            return this.GetOrCreateContext(assemblyPath,
                                      project,
                                      new ClrTypeName(type.FullyQualifiedName),
                                      type.GetSubjectString(),
                                      type.GetTags(), type.IsIgnored());
        }

        public ContextElement GetOrCreateContext(string assemblyPath,
                                                 IProject project,
                                                 IClrTypeName typeName,
                                                 string subject,
                                                 ICollection<string> tags,
                                                 bool isIgnored)
        {
            UnitTestElementId id = ContextElement.CreateId(_elementIdFactory, _provider, project, subject, typeName.FullName, tags);
            var contextElement = this._manager.GetElementById(id) as ContextElement;
            if (contextElement != null)
            {
                contextElement.State = UnitTestElementState.Valid;
                contextElement.AssemblyLocation = assemblyPath;
                return contextElement;
            }

            return new ContextElement(this._provider,
                                      id,
                                      typeName,
                                      this._cachingService,
                                      assemblyPath,
                                      subject,
                                      tags,
                                      isIgnored,
                                      _categoryFactory);
        }

        public void UpdateChildState(ITypeElement type)
        {
            var context = this._cache.TryGetContext(type);
            if (context == null)
            {
                return;
            }

            foreach (var element in context
              .Children.Where(x => x.State == UnitTestElementState.Pending)
              .Flatten(x => x.Children))
            {
                element.State = UnitTestElementState.Invalid;
            }
        }
    }
}