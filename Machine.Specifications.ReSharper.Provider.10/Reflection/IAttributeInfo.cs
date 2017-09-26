using System.Collections.Generic;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public interface IAttributeInfo
    {
        IEnumerable<string> GetParameters();
    }
}
