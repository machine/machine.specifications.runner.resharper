using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Factories;

namespace Machine.Specifications.ReSharperProvider.Explorers.ElementHandlers
{
    public class ContextSpecificationElementHandler : IElementHandler
    {
        private readonly ContextSpecificationFactory _factory;
        private readonly IUnitTestElementsObserver _consumer;

        public ContextSpecificationElementHandler(ElementFactories factories, IUnitTestElementsObserver consumer)
        {
            _factory = factories.ContextSpecifications;
            _consumer = consumer;
        }

        public bool Accepts(ITreeNode element)
        {
            var declaration = element as IDeclaration;

            if (declaration == null)
                return false;

            return declaration.DeclaredElement.IsSpecification();
        }

        public IEnumerable<UnitTestElementDisposition> AcceptElement(string assemblyPath, IFile file, ITreeNode element)
        {
            var declaration = (IDeclaration)element;
            var contextSpecification = _factory.CreateContextSpecification(_consumer, declaration.DeclaredElement);

            if (contextSpecification == null)
                yield break;

            yield return new UnitTestElementDisposition(contextSpecification, file.GetSourceFile().ToProjectFile(),
                declaration.GetNameDocumentRange().TextRange, declaration.GetDocumentRange().TextRange);
        }
    }
}