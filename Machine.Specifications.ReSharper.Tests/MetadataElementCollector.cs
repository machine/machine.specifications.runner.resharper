using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider;
using Machine.Specifications.ReSharperProvider.Reflection;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharper.Tests
{
    public class MetadataElementCollector : ICollector
    {
        public IList<ITypeInfo> Types { get; } = new List<ITypeInfo>();

        public IList<IFieldInfo> Fields { get; } = new List<IFieldInfo>();

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
    }
}