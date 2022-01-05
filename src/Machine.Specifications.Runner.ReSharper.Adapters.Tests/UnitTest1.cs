using System;
using System.IO;
using System.Linq;
using System.Threading;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery;
using Xunit;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var path = @"C:\Projects\MSpecProject\MSpecProject\bin\Debug\net472\MSpecProject.dll";

            File.Copy(path, Path.Combine(Environment.CurrentDirectory, Path.GetFileName(path)), true);

            var controller = new MspecController(CancellationToken.None);
            var sink = new MspecDiscoverySink();

            controller.Find(sink, path);
        }
    }
}
