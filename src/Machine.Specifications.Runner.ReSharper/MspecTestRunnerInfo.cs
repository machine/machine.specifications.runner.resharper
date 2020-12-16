using System.Reflection;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper
{
    public static class MspecTestRunnerInfo
    {
        public static readonly FileSystemPath Root = Assembly.GetExecutingAssembly().GetPath().Directory;
    }
}
