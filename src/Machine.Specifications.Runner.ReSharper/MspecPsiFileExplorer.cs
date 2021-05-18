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
using Machine.Specifications.Runner.ReSharper.Reflection;

namespace Machine.Specifications.Runner.ReSharper
{
    public class MspecPsiFileExplorer : IRecursiveElementProcessor
    {
        private readonly SearchDomainFactory searchDomainFactory;

        private readonly UnitTestElementFactory factory;

        private readonly IUnitTestElementsObserver observer;

        private readonly Func<bool> interrupted;

        private readonly Dictionary<ClrTypeName, IUnitTestElement> recentContexts = new();

        public MspecPsiFileExplorer(SearchDomainFactory searchDomainFactory, UnitTestElementFactory factory, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            this.searchDomainFactory = searchDomainFactory;
            this.factory = factory;
            this.observer = observer;
            this.interrupted = interrupted;
        }

        public bool ProcessingIsFinished
        {
            get
            {
                if (interrupted())
                {
                    throw new OperationCanceledException();
                }

                return false;
            }
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            if (element is ITypeMemberDeclaration)
            {
                return element is ITypeDeclaration;
            }

            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            if (element is not IDeclaration declaration)
            {
                return;
            }

            var declaredElement = declaration.DeclaredElement;

            if (declaredElement is IClass type)
            {
                ProcessType(type.AsTypeInfo(), declaredElement, declaration);
            }
            else if (declaredElement is IField field)
            {
                ProcessField(declaration.GetProject()!, field.AsFieldInfo(), declaration);
            }
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        private void ProcessType(ITypeInfo type, IDeclaredElement element, IDeclaration declaration)
        {
            if (type.IsContext())
            {
                ProcessContext(type, declaration, true);
            }
            else if (type.IsBehaviorContainer())
            {
                ProcessBehaviorContainer(element);
            }
        }

        private void ProcessContext(ITypeInfo type, IDeclaration declaration, bool isClear)
        {
            var name = new ClrTypeName(type.FullyQualifiedName);
            var project = declaration.GetProject()!;

            var context = factory.GetOrCreateContext(
                project,
                name,
                type.GetSubject(),
                type.GetTags().ToArray(),
                type.GetIgnoreReason());

            recentContexts[name] = context;

            if (isClear)
            {
                OnUnitTestElement(context, declaration);
            }
        }

        private void ProcessBehaviorContainer(IDeclaredElement element)
        {
            var solution = element.GetSolution();

            var finder = solution.GetPsiServices().Finder;
            var searchDomain = searchDomainFactory.CreateSearchDomain(solution, false);
            var consumer = new SearchResultsConsumer();
            var progress = NullProgressIndicator.Create();

            finder.Find(new[] {element}, searchDomain, consumer, SearchPattern.FIND_USAGES, progress);

            var contexts = consumer.GetOccurrences()
                .OfType<ReferenceOccurrence>()
                .Select(x => x.GetTypeElement()?.GetValidDeclaredElement())
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
                    {
                        ProcessField(declaration.GetProject()!, field);
                    }
                }
            }
        }

        private void ProcessField(IProject project, IFieldInfo field, IDeclaration? declaration = null)
        {
            if (field.IsSpecification())
            {
                ProcessSpecificationField(project, field, declaration);
            }
            else if (field.IsBehavior())
            {
                ProcessBehaviorField(project, field, declaration);
            }
        }

        private void ProcessSpecificationField(IProject project, IFieldInfo field, IDeclaration? declaration = null)
        {
            var containingType = new ClrTypeName(field.DeclaringType);

            if (recentContexts.TryGetValue(containingType, out var context))
            {
                var specification = factory.GetOrCreateContextSpecification(
                    project,
                    context,
                    containingType,
                    field.ShortName,
                    field.GetIgnoreReason());

                OnUnitTestElement(specification, declaration);
            }
        }

        private void ProcessBehaviorField(IProject project, IFieldInfo field, IDeclaration? declaration = null)
        {
            var behaviorType = field.FieldType.GetGenericArguments().FirstOrDefault();
            var containingType = new ClrTypeName(field.DeclaringType);

            if (recentContexts.TryGetValue(containingType, out var context))
            {
                var behavior = factory.GetOrCreateBehavior(
                    project,
                    context,
                    containingType,
                    field.ShortName,
                    field.GetIgnoreReason());

                OnUnitTestElement(behavior, declaration);

                if (behaviorType != null && behaviorType.IsBehaviorContainer())
                {
                    var specFields = behaviorType.GetFields()
                        .Where(x => x.IsSpecification());

                    foreach (var specField in specFields)
                    {
                        var specification = factory.GetOrCreateBehaviorSpecification(
                            project,
                            behavior,
                            containingType,
                            specField.ShortName,
                            specField.GetIgnoreReason());

                        OnUnitTestElement(specification, declaration);
                    }
                }
            }
        }

        private void OnUnitTestElement(IUnitTestElement element, IDeclaration? declaration = null)
        {
            if (declaration == null)
            {
                observer.OnUnitTestElementDisposition(UnitTestElementDisposition.NotYetClear(element));
                return;
            }

            var project = declaration.GetSourceFile().ToProjectFile();
            var textRange = declaration.GetNameDocumentRange().TextRange;
            var containingRange = declaration.GetDocumentRange().TextRange;

            var disposition = new UnitTestElementDisposition(element, project, textRange, containingRange);

            if (textRange.IsValid && containingRange.IsValid)
            {
                observer.OnUnitTestElementDisposition(disposition);
            }
            else
            {
                observer.OnUnitTestElement(element);
            }
        }
    }
}
