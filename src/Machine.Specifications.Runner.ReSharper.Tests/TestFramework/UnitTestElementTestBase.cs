using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework
{
    [TestFixture]
    public abstract class UnitTestElementTestBase
    {
        protected void WithDiscovery(Action action)
        {
            var project = Substitute.For<IProject>();
            var provider = Substitute.For<IUnitTestProvider>();

            var target = new ExplorationTarget(project, TargetFrameworkId.Default, provider);
            var source = new UnitTestElementSource(UnitTestElementOrigin.Source, target);

            using (DiscoveryContext.As(source))
            {
                action();
            }
        }
    }
}
