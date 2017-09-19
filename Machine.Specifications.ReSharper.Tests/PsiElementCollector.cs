using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace Machine.Specifications.ReSharper.Tests
{
    public class PsiElementCollector : IRecursiveElementProcessor
    {
        public List<IClass> Classes { get; } = new List<IClass>();

        public List<IField> Fields { get; } = new List<IField>();

        public bool ProcessingIsFinished => false;

        public void Reset()
        {
            Classes.Clear();
            Fields.Clear();
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            if (!(element is IDeclaration declaration))
                return;

            if (declaration.DeclaredElement is IClass classElement)
                Classes.Add(classElement);

            if (declaration.DeclaredElement is IField fieldElement)
                Fields.Add(fieldElement);
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }
    }
}
