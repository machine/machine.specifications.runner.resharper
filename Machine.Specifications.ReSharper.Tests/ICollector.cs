using System.Collections.Generic;
using Machine.Specifications.ReSharperProvider.Reflection;

namespace Machine.Specifications.ReSharper.Tests
{
    public interface ICollector
    {
        IList<ITypeInfo> Types { get; }

        IList<IFieldInfo> Fields { get; }
    }
}