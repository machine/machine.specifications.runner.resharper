using System;
using JetBrains.Metadata.Access;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper
{
    public class MspecTaskRunnerRuntimeEnvironment : TaskRunnerRuntimeEnvironment
    {
        public MspecTaskRunnerRuntimeEnvironment(
            IProject project,
            TargetPlatform targetPlatform,
            TargetFrameworkId targetFrameworkId,
            bool unmanaged,
            PlatformMonoPreference platformMonoPreference)
            : base(targetPlatform, targetFrameworkId, unmanaged, platformMonoPreference)
        {
            Project = project;
        }

        public IProject Project { get; }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Project.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TaskRunnerRuntimeEnvironment);
        }

        public bool Equals(MspecTaskRunnerRuntimeEnvironment obj)
        {
            return Equals((TaskRunnerRuntimeEnvironment) obj) && Equals(obj.Project, Project);
        }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine + "  Project: " + Project.Name;
        }
    }
}
