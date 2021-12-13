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
        private readonly IUnitTestElementObserverOnFile observer;

        private readonly Func<bool> interrupted;

        private readonly UnitTestElementFactory factory = new();

        private readonly Dictionary<ClrTypeName, MspecContextTestElement> recentContexts = new();

        public MspecPsiFileExplorer(IUnitTestElementObserverOnFile observer, Func<bool> interrupted)
        {
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
