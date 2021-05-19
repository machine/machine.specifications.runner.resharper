using System;
using System.IO;
using System.Threading;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

[assembly: RequiresThread(ApartmentState.STA)]

#pragma warning disable CS0618
[assembly: TestDataPathBase("Data")]
#pragma warning restore CS0618

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SetUpFixture]
    public class TestEnvironment : ExtensionTestEnvironmentAssembly<IMspecTestZone>
    {
        static TestEnvironment()
        {
            try
            {
                //SetJetTestPackagesDir();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SetJetTestPackagesDir()
        {
            if (Environment.GetEnvironmentVariable("JET_TEST_PACKAGES_DIR") == null)
            {
                var packages = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "JetTestPackages");

                if (!Directory.Exists(packages))
                {
                    TestUtil.SetHomeDir(typeof(TestEnvironment).Assembly);

                    var testData = TestUtil.GetTestDataPathBase(typeof(TestEnvironment).Assembly);

                    packages = testData.Parent.Combine("JetTestPackages").FullPath;
                }

                Environment.SetEnvironmentVariable("JET_TEST_PACKAGES_DIR", packages, EnvironmentVariableTarget.Process);
            }
        }
    }
}
