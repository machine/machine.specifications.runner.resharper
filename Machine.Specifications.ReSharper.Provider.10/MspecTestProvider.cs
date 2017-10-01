using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util.Reflection;
using Machine.Specifications.ReSharperProvider.Presentation;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider
{
    [UnitTestProvider]
    public class MspecTestProvider : IUnitTestProvider
    {
        private static readonly AssemblyNameInfo MSpecReferenceName = AssemblyNameInfoFactory.Create2(MspecTaskRunner.RunnerId, null);

        private readonly UnitTestElementComparer _unitTestElementComparer = new UnitTestElementComparer(
            typeof(ContextElement),
            typeof(BehaviorElement),
            typeof(BehaviorSpecificationElement),
            typeof(ContextSpecificationElement));

        public string ID => MspecTaskRunner.RunnerId;

        public string Name => ID;

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            return _unitTestElementComparer.Compare(x, y);
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Test:
                    return element is ContextSpecificationElement || element is BehaviorSpecificationElement;

                case UnitTestElementKind.TestContainer:
                    return element is ContextElement || element is BehaviorElement;

                case UnitTestElementKind.TestStuff:
                    return element is Element;

                case UnitTestElementKind.Unknown:
                    return !(element is Element);
            }

            return false;
        }

        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Test:
                    return declaredElement.IsSpecification();

                case UnitTestElementKind.TestContainer:
                    return declaredElement.IsContext() || declaredElement.IsBehavior();

                case UnitTestElementKind.TestStuff:
                    return declaredElement.IsSpecification() || declaredElement.IsContext() || declaredElement.IsBehavior();

                case UnitTestElementKind.Unknown:
                    return !(declaredElement.IsSpecification() || declaredElement.IsContext() || declaredElement.IsBehavior());
            }

            return false;
        }

        public bool IsSupported(IHostProvider hostProvider, IProject project, TargetFrameworkId targetFrameworkId)
        {
            return IsSupported(project, targetFrameworkId);
        }

        public bool IsSupported(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return ReferencedAssembliesService.IsProjectReferencingAssemblyByName(project, targetFrameworkId, MSpecReferenceName, out AssemblyNameInfo _);
        }
    }
}