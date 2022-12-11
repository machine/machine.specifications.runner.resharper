using System.Collections.Generic;

namespace Machine.Specifications.Runner.ReSharper.Reflection;

public interface IAttributeInfo
{
    IEnumerable<string> GetParameters();
}
