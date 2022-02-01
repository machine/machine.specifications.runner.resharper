using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.ReSharper.UnitTestFramework.Execution.Hosting;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using JetBrains.Util.Reflection;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Reflection;

namespace Machine.Specifications.Runner.ReSharper
{
    [UnitTestProvider]
    public class MspecTestProvider : IDotNetArtifactBasedUnitTestProvider
    {
        public const string Id = "Machine.Specifications";

        private static readonly AssemblyNameInfo MspecReferenceName = AssemblyNameInfoFactory.Create2(Id, null);

        public string ID => Id;

        public string Name => Id;

        public IUnitTestRunStrategy GetRunStrategy(IUnitTestElement element, IHostProvider hostProvider)
        {
            return UT.Facade.Get<MspecServiceProvider>().GetRunStrategy();
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Test:
                    return element is MspecSpecificationTestElement or MspecBehaviorSpecificationTestElement;

                case UnitTestElementKind.TestContainer:
                    return element is MspecContextTestElement;

                case UnitTestElementKind.TestStuff:
                    return element is MspecContextTestElement or MspecSpecificationTestElement or MspecBehaviorSpecificationTestElement;

                case UnitTestElementKind.Unknown:
                    return element is not MspecContextTestElement &&
                           element is not MspecSpecificationTestElement &&
                           element is not MspecBehaviorSpecificationTestElement;
            }

            return false;
        }

        public bool IsElementOfKind(IDeclaredElement element, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Test:
                    return element.IsSpecification();

                case UnitTestElementKind.TestContainer:
                    return element.IsContext();

                case UnitTestElementKind.TestStuff:
                    return element.IsSpecification() || element.IsContext() || element.IsBehavior();

                case UnitTestElementKind.Unknown:
                    return !(element.IsSpecification() || element.IsContext() || element.IsBehavior());
            }

            return false;
        }

        public bool IsSupported(IHostProvider hostProvider, IProject project, TargetFrameworkId targetFrameworkId)
        {
            return IsSupported(project, targetFrameworkId);
        }

        public bool IsSupported(IProject project, TargetFrameworkId targetFrameworkId)
        {
            using (ReadLockCookie.Create())
            {
                return ReferencedAssembliesService.IsProjectReferencingAssemblyByName(project, targetFrameworkId, MspecReferenceName, out _);
            }
        }

        public bool SupportsResultEventsForParentOf(IUnitTestElement element)
        {
            return element is not MspecContextTestElement || element.Parent is not MspecContextTestElement;
        }
    }
}
