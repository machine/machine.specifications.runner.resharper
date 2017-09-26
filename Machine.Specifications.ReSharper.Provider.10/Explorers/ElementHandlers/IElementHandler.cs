using System.Collections.Generic;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.Explorers.ElementHandlers
{
    internal interface IElementHandler
    {
        bool Accepts(ITreeNode element);

        IEnumerable<UnitTestElementDisposition> AcceptElement(FileSystemPath assemblyPath, IFile file, ITreeNode element);
    }
}