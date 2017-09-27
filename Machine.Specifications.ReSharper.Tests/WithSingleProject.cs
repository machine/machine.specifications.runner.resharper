using System;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider;

namespace Machine.Specifications.ReSharper.Tests
{
    public abstract class WithSingleProject : BaseTestWithSingleProject
    {
        protected override string RelativeTestDataPath => string.Empty;

        protected MspecServiceProvider ServiceProvider => 
            Solution.GetComponent<MspecServiceProvider>();

        protected IProject Project => Solution.GetProjectsByName("TestProject").FirstOrDefault();

        protected UnitTestElementId CreateId(string id)
        {
            var factory = Solution.GetComponent<IUnitTestElementIdFactory>();

            return factory.Create("Provider", "Project", TargetFrameworkId.Default, id);
        }

        protected void With(Action action)
        {
            WithSingleProject("none.cs", (lifetime, solution, project) =>
            {
                Locks.ReentrancyGuard.Execute(GetType().Name, () =>
                {
                    using (ReadLockCookie.Create())
                        action();
                });
            });
        }
    }
}