using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SolutionComponent]
    public class UnitTestArtifactResolver : IUnitTestProjectArtifactResolverCollaborator
    {
        public bool CanResolveArtifact(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return true;
        }

        public FileSystemPath ResolveArtifact(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return project.GetSubItems()[0].Location;
        }
    }
}
