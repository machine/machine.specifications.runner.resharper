using System;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var path = @"C:\Users\rober\source\repos\MSpecProject\MspecBehaviors\bin\Debug\net472\MspecBehaviors.dll";

            //File.Copy(path, Path.Combine(Environment.CurrentDirectory, Path.GetFileName(path)));

            var controller = new MspecController(CancellationToken.None);
            var results = controller.Find(path).ToArray();
        }
    }
}
