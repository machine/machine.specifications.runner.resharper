namespace Machine.Specifications.ReSharperProvider.Explorers.ElementHandlers
{
    using System.Collections.Generic;

    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Factories;

    class ContextSpecificationElementHandler : IElementHandler
    {
        readonly ContextSpecificationFactory _factory;
        readonly IUnitTestElementsObserver _consumer;

        public ContextSpecificationElementHandler(ElementFactories factories, IUnitTestElementsObserver consumer)
        {
            this._factory = factories.ContextSpecifications;
            _consumer = consumer;
        }

        public bool Accepts(ITreeNode element)
        {
            var declaration = element as IDeclaration;
            if (declaration == null)
            {
                return false;
            }

            return declaration.DeclaredElement.IsSpecification();
        }

        public IEnumerable<UnitTestElementDisposition> AcceptElement(string assemblyPath, IFile file, ITreeNode element)
        {
            var declaration = (IDeclaration)element;
            var contextSpecification = this._factory.CreateContextSpecification(_consumer, declaration.DeclaredElement);

            if (contextSpecification == null)
            {
                yield break;
            }

            yield return new UnitTestElementDisposition(contextSpecification,
                                                        file.GetSourceFile().ToProjectFile(),
                                                        declaration.GetNameDocumentRange().TextRange,
                                                        declaration.GetDocumentRange().TextRange);
        }

        public void Cleanup(ITreeNode element)
        {
        }
    }
}