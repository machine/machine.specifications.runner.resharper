using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Factories;

namespace Machine.Specifications.ReSharperProvider.Explorers.ElementHandlers
{
    public class ContextElementHandler : IElementHandler
    {
        private readonly ContextFactory _factory;
        private readonly IUnitTestElementsObserver _consumer;

        public ContextElementHandler(ElementFactories factories, IUnitTestElementsObserver consumer)
        {
            _factory = factories.Contexts;
            _consumer = consumer;
        }

        public bool Accepts(ITreeNode element)
        {
            var declaration = element as IDeclaration;

            if (declaration == null)
                return false;

            return declaration.DeclaredElement.IsContext();
        }

        public IEnumerable<UnitTestElementDisposition> AcceptElement(string assemblyPath, IFile file, ITreeNode element)
        {
            var declaration = (IDeclaration)element;
            var context = _factory.CreateContext(_consumer, assemblyPath, declaration);

            if (context == null)
                yield break;

            yield return new UnitTestElementDisposition(context, file.GetSourceFile().ToProjectFile(),
                declaration.GetNameDocumentRange().TextRange, declaration.GetDocumentRange().TextRange);
        }
    }
}