﻿using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using JetBrains.Util.Reflection;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Runner;

namespace Machine.Specifications.Runner.ReSharper
{
    [UnitTestProvider]
    public class MspecTestProvider : IDotNetArtifactBasedUnitTestProvider
    {
        private static readonly AssemblyNameInfo MSpecReferenceName = AssemblyNameInfoFactory.Create2(MspecTaskRunner.RunnerId, null);

        public string ID => MspecTaskRunner.RunnerId;

        public string Name => MspecTaskRunner.RunnerId;

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Test:
                    return element is MspecContextSpecificationTestElement or MspecBehaviorSpecificationTestElement;

                case UnitTestElementKind.TestContainer:
                    return element is MspecContextTestElement or MspecBehaviorTestElement;

                case UnitTestElementKind.TestStuff:
                    return element is MspecTestElement;

                case UnitTestElementKind.Unknown:
                    return element is not MspecTestElement;
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
            using (ReadLockCookie.Create())
            {
                return ReferencedAssembliesService.IsProjectReferencingAssemblyByName(project, targetFrameworkId, MSpecReferenceName, out _);
            }
        }

        public bool SupportsResultEventsForParentOf(IUnitTestElement element)
        {
            return true;
        }
    }
}
