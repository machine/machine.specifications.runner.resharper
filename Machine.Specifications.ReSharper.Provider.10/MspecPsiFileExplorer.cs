using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Reflection;

namespace Machine.Specifications.ReSharperProvider
{
    public class MspecPsiFileExplorer : IRecursiveElementProcessor
    {
        private readonly SearchDomainFactory _searchDomainFactory;
        private readonly UnitTestElementFactory _factory;
        private readonly IUnitTestElementsObserver _observer;
        private readonly Func<bool> _interrupted;

        private readonly Dictionary<ClrTypeName, IUnitTestElement> _contexts = new Dictionary<ClrTypeName, IUnitTestElement>();

        public MspecPsiFileExplorer(SearchDomainFactory searchDomainFactory, UnitTestElementFactory factory, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            _searchDomainFactory = searchDomainFactory;
            _factory = factory;
            _observer = observer;
            _interrupted = interrupted;
        }

        public bool ProcessingIsFinished
        {
            get
            {
                if (_interrupted())
                    throw new OperationCanceledException();

                return false;
            }
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            if (element is ITypeMemberDeclaration)
                return element is ITypeDeclaration;

            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            if (!(element is IDeclaration declaration))
                return;

            var declaredElement = declaration.DeclaredElement;

            if (declaredElement is IClass type)
                ProcessType(type.AsTypeInfo(), declaredElement, declaration);
            else if (declaredElement is IField field)
                ProcessField(declaration.GetProject(), field.AsFieldInfo(), declaration);
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        private void ProcessType(ITypeInfo type, IDeclaredElement element, IDeclaration declaration)
        {
            if (type.IsContext())
                ProcessContext(type, declaration, true);
            else if (type.IsBehaviorContainer())
                ProcessBehaviorContainer(element);
        }

        private void ProcessContext(ITypeInfo type, IDeclaration declaration, bool isClear)
        {
            var name = new ClrTypeName(type.FullyQualifiedName);
            var project = declaration.GetProject();
            var assemblyPath = project?.GetOutputFilePath(_observer.TargetFrameworkId);

            var context = _factory.GetOrCreateContext(
                project,
                name,
                assemblyPath,
                type.GetSubject(),
                type.GetTags().ToArray(),
                type.IsIgnored(),
                UnitTestElementCategorySource.Source,
                out _);

            _contexts[name] = context;

            if (isClear)
                OnUnitTestElement(context, declaration);
        }

        private void ProcessBehaviorContainer(IDeclaredElement element)
        {
            var solution = element.GetSolution();

            var finder = solution.GetPsiServices().Finder;
            var searchDomain = _searchDomainFactory.CreateSearchDomain(solution, false);
            var consumer = new SearchResultsConsumer();
            var progress = NullProgressIndicator.Create();

            finder.Find(new[] {element}, searchDomain, consumer, SearchPattern.FIND_USAGES, progress);

            var contexts = consumer.GetOccurrences()
                .OfType<ReferenceOccurrence>()
                .Select(x => x.GetTypeElement().GetValidDeclaredElement())
                .OfType<IClass>()
                .Where(x => x.IsContext());

            foreach (var context in contexts)
            {
                var type = context.AsTypeInfo();
                var declaration = context.GetDeclarations().FirstOrDefault();

                if (declaration != null)
                {
                    ProcessContext(type, declaration, false);

                    foreach (var field in type.GetFields())
                        ProcessField(declaration.GetProject(), field);
                }
            }
        }

        private void ProcessField(IProject project, IFieldInfo field, IDeclaration declaration = null)
        {
            if (field.IsSpecification())
                ProcessSpecificationField(project, field, declaration);
            else if (field.IsBehavior())
                ProcessBehaviorField(project, field, declaration);
        }

        private void ProcessSpecificationField(IProject project, IFieldInfo field, IDeclaration declaration = null)
        {
            var containingType = new ClrTypeName(field.DeclaringType);

            if (_contexts.TryGetValue(containingType, out var context))
            {
                var specification = _factory.GetOrCreateContextSpecification(
                    project,
                    context,
                    containingType,
                    field.ShortName,
                    field.IsIgnored());

                OnUnitTestElement(specification, declaration);
            }
        }

        private void ProcessBehaviorField(IProject project, IFieldInfo field, IDeclaration declaration = null)
        {
            var behaviorType = field.FieldType.GetGenericArguments().FirstOrDefault();
            var containingType = new ClrTypeName(field.DeclaringType);

            if (_contexts.TryGetValue(containingType, out var context))
            {
                var behavior = _factory.GetOrCreateBehavior(
                    project,
                    context,
                    containingType,
                    field.ShortName,
                    field.IsIgnored());

                OnUnitTestElement(behavior, declaration);

                if (behaviorType != null && behaviorType.IsBehaviorContainer())
                {
                    var specFields = behaviorType.GetFields()
                        .Where(x => x.IsSpecification());

                    foreach (var specField in specFields)
                    {
                        var specification = _factory.GetOrCreateBehaviorSpecification(
                            project,
                            behavior,
                            containingType,
                            specField.ShortName,
                            specField.IsIgnored());

                        OnUnitTestElement(specification, declaration);
                    }
                }
            }
        }

        private void OnUnitTestElement(IUnitTestElement element, IDeclaration declaration = null)
        {
            if (declaration == null)
            {
                _observer.OnUnitTestElementDisposition(UnitTestElementDisposition.NotYetClear(element));
                return;
            }

            var project = declaration.GetSourceFile().ToProjectFile();
            var textRange = declaration.GetNameDocumentRange().TextRange;
            var containingRange = declaration.GetDocumentRange().TextRange;

            var disposition = new UnitTestElementDisposition(element, project, textRange, containingRange);

            _observer.OnUnitTestElementDisposition(disposition);
        }
    }
}
