using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Reflection;

namespace Machine.Specifications.Runner.ReSharper
{
    public class MspecPsiFileExplorer : IRecursiveElementProcessor
    {
        private readonly SearchDomainFactory searchDomainFactory;

        private readonly IUnitTestElementObserverOnFile observer;

        private readonly Func<bool> interrupted;

        private readonly UnitTestElementFactory factory = new();

        private readonly Dictionary<ClrTypeName, MspecContextTestElement> recentContexts = new();

        public MspecPsiFileExplorer(SearchDomainFactory searchDomainFactory, IUnitTestElementObserverOnFile observer, Func<bool> interrupted)
        {
            this.searchDomainFactory = searchDomainFactory;
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
                ProcessField(field.AsFieldInfo(), declaration);
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

            var context = factory.GetOrCreateContext(
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
                        ProcessField(field);
                    }
                }
            }
        }

        private void ProcessField(IFieldInfo field, IDeclaration? declaration = null)
        {
            if (field.IsSpecification())
            {
                ProcessSpecificationField(field, declaration);
            }
            else if (field.IsBehavior())
            {
                ProcessBehaviorField(field, declaration);
            }
        }

        private void ProcessSpecificationField(IFieldInfo field, IDeclaration? declaration = null)
        {
            var containingType = new ClrTypeName(field.DeclaringType);

            if (recentContexts.TryGetValue(containingType, out var context))
            {
                var specification = factory.GetOrCreateContextSpecification(
                    context,
                    field.ShortName,
                    field.GetIgnoreReason());

                OnUnitTestElement(specification, declaration);
            }
        }

        private void ProcessBehaviorField(IFieldInfo field, IDeclaration? declaration = null)
        {
            var behaviorType = field.FieldType.GetGenericArguments().FirstOrDefault();
            var containingType = new ClrTypeName(field.DeclaringType);

            if (recentContexts.TryGetValue(containingType, out var context))
            {
                var behavior = factory.GetOrCreateBehavior(
                    context,
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
                            behavior,
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
                return;
            }

            var navigationRange = declaration.GetNameDocumentRange().TextRange;
            var containingRange = declaration.GetDocumentRange().TextRange;

            observer.OnUnitTestElement(element);
            observer.OnUnitTestElementDisposition(element, navigationRange, containingRange);
        }
    }
}
