using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider;
using Machine.Specifications.ReSharperProvider.Reflection;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharper.Tests
{
    public class MspecElementCollector : IRecursiveElementProcessor, ICollector
    {
        public IList<ITypeInfo> Types { get; } = new List<ITypeInfo>();

        public IList<IFieldInfo> Fields { get; } = new List<IFieldInfo>();

        public bool ProcessingIsFinished => false;

        public void Reset()
        {
            Types.Clear();
            Fields.Clear();
        }

        public void Explore(IMetadataAssembly assembly)
        {
            using (ReadLockCookie.Create())
            {
                var types = assembly.GetTypes()
                    .Flatten(x => x.GetNestedTypes())
                    .Where(x => x.FullyQualifiedName != "<Module>");

                Types.AddRange(types.Select(x => x.AsTypeInfo()));

                foreach (var type in Types)
                    Fields.AddRange(type.GetFields());
            }
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
                Types.Add(classElement.AsTypeInfo());

            if (declaration.DeclaredElement is IField fieldElement)
                Fields.Add(fieldElement.AsFieldInfo());
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }
    }
}
