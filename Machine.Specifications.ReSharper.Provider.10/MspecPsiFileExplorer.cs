using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Reflection;

namespace Machine.Specifications.ReSharperProvider
{
    public class MspecPsiFileExplorer : IRecursiveElementProcessor
    {
        private readonly UnitTestElementFactory _factory;
        private readonly IFile _file;
        private readonly IUnitTestElementsObserver _observer;
        private readonly Func<bool> _interrupted;

        private readonly Dictionary<ClrTypeName, IUnitTestElement> _contexts = new Dictionary<ClrTypeName, IUnitTestElement>();

        public MspecPsiFileExplorer(UnitTestElementFactory factory, IFile file, IUnitTestElementsObserver observer, Func<bool> interrupted)
        {
            _factory = factory;
            _file = file;
            _observer = observer;
            _interrupted = interrupted;
        }

        public bool ProcessingIsFinished
        {
            get
            {
                if (_interrupted())
                    throw new ProcessCancelledException();

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
                ProcessType(type.AsTypeInfo(), declaration);
            else if (declaredElement is IField field)
                ProcessField(field.AsFieldInfo(), declaration);
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        private void ProcessType(ITypeInfo type, IDeclaration declaration)
        {
            if (!type.IsContext())
                return;

            var name = new ClrTypeName(type.FullyQualifiedName);
            var assemblyPath = _file.GetProject()?.GetOutputFilePath(_observer.TargetFrameworkId);

            var context = _factory.GetOrCreateContext(
                name,
                assemblyPath,
                type.GetSubject(),
                type.GetTags().ToArray(),
                type.IsIgnored());

            _contexts[name] = context;

            OnUnitTestElement(context, declaration);
        }

        private void ProcessField(IFieldInfo field, IDeclaration declaration)
        {
            if (field.IsSpecification())
                ProcessSpecificationField(field, declaration);
            else if (field.IsBehavior())
                ProcessBehaviorField(field, declaration);
        }

        private void ProcessSpecificationField(IFieldInfo field, IDeclaration declaration)
        {
            var containingType = new ClrTypeName(field.DeclaringType);

            if (_contexts.TryGetValue(containingType, out var context))
            {
                var specification = _factory.GetOrCreateContextSpecification(
                    context,
                    containingType,
                    field.ShortName,
                    field.IsIgnored());

                OnUnitTestElement(specification, declaration);
            }
        }

        private void ProcessBehaviorField(IFieldInfo field, IDeclaration declaration)
        {
            var behaviorType = field.FieldType.GetGenericArguments().FirstOrDefault();
            var containingType = new ClrTypeName(field.DeclaringType);

            if (_contexts.TryGetValue(containingType, out var context))
            {
                var behavior = _factory.GetOrCreateBehavior(
                    context,
                    containingType,
                    field.ShortName,
                    behaviorType?.FullyQualifiedName,
                    field.IsIgnored());

                OnUnitTestElement(behavior, declaration);

                if (behaviorType != null && behaviorType.IsBehaviorContainer())
                {
                    var specFields = behaviorType.GetFields()
                        .Where(x => x.IsSpecification());

                    foreach (var specField in specFields)
                    {
                        var specification = _factory.GetOrCreateBehaviorSpecification(
                            behavior,
                            containingType,
                            specField.ShortName,
                            specField.IsIgnored());

                        OnUnitTestElement(specification, declaration);
                    }
                }
            }
        }

        private void OnUnitTestElement(IUnitTestElement element, IDeclaration declaration)
        {
            var project = _file.GetSourceFile().ToProjectFile();
            var textRange = declaration.GetNameDocumentRange().TextRange;
            var containingRange = declaration.GetDocumentRange().TextRange;

            var disposition = new UnitTestElementDisposition(element, project, textRange, containingRange);

            _observer.OnUnitTestElementDisposition(disposition);
        }
    }
}
