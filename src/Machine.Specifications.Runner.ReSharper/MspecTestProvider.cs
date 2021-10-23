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
            return elementKind switch
            {
                UnitTestElementKind.Test => element is MspecContextSpecificationTestElement or MspecBehaviorSpecificationTestElement,
                UnitTestElementKind.TestContainer => element is MspecContextTestElement or MspecBehaviorTestElement,
                UnitTestElementKind.TestStuff => element is MspecContextTestElement or MspecBehaviorTestElement or MspecContextSpecificationTestElement or MspecBehaviorSpecificationTestElement,
                UnitTestElementKind.Unknown => element is not MspecContextTestElement &&
                                               element is not MspecBehaviorTestElement &&
                                               element is not MspecContextSpecificationTestElement &&
                                               element is not MspecBehaviorSpecificationTestElement,
                _ => false
            };
        }

        public bool IsElementOfKind(IDeclaredElement element, UnitTestElementKind elementKind)
        {
            return elementKind switch
            {
                UnitTestElementKind.Test => element.IsSpecification(),
                UnitTestElementKind.TestContainer => element.IsContext() || element.IsBehavior(),
                UnitTestElementKind.TestStuff => element.IsSpecification() || element.IsContext() || element.IsBehavior(),
                UnitTestElementKind.Unknown => !(element.IsSpecification() || element.IsContext() || element.IsBehavior()),
                _ => false
            };
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
