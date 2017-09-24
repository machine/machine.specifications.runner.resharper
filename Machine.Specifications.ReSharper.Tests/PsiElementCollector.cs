using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using Machine.Specifications.ReSharperProvider;
using Machine.Specifications.ReSharperProvider.Reflection;

namespace Machine.Specifications.ReSharper.Tests
{
    public class PsiElementCollector : IRecursiveElementProcessor, ICollector
    {
        public IList<ITypeInfo> Types { get; } = new List<ITypeInfo>();

        public IList<IFieldInfo> Fields { get; } = new List<IFieldInfo>();

        public bool ProcessingIsFinished => false;

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            if (!(element is IDeclaration declaration))
                return;

            if (declaration.DeclaredElement is IClass classElement)
                Types.Add(classElement.AsTypeInfo());

            if (declaration.DeclaredElement is IField fieldElement)
                Fields.Add(fieldElement.AsFieldInfo());
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }
    }
}
