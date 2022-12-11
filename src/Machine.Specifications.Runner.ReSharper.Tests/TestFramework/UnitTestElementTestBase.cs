using System;
using System.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework;

[TestFixture]
public abstract class UnitTestElementTestBase
{
    [SetUp]
    public void SetUp()
    {
        var factoryMethod = typeof(Logger).GetProperty(nameof(Logger.Factory), BindingFlags.Static | BindingFlags.Public);

        if (factoryMethod != null)
        {
            var factory = factoryMethod.GetValue(null);

            if (factory == null)
            {
                factoryMethod.SetValue(null, new LoggerFactory());
            }
        }
    }

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
