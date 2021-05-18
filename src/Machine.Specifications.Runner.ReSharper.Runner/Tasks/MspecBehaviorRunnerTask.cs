using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecBehaviorRunnerTask : MspecRunnerTask, IEquatable<MspecBehaviorRunnerTask>
    {
        public MspecBehaviorRunnerTask(string projectId, string contextTypeName, string behaviorFieldName, string? ignoreReason)
            : base(ignoreReason)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
            BehaviorFieldName = behaviorFieldName;
        }

        public MspecBehaviorRunnerTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
            ContextTypeName = GetXmlAttribute(element, nameof(ContextTypeName));
            BehaviorFieldName = GetXmlAttribute(element, nameof(BehaviorFieldName));
        }

        public string ProjectId { get; }

        public string ContextTypeName { get; }

        public string BehaviorFieldName { get; }

        public override bool IsMeaningfulTask => false;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, nameof(ProjectId), ProjectId);
            SetXmlAttribute(element, nameof(ContextTypeName), ContextTypeName);
            SetXmlAttribute(element, nameof(BehaviorFieldName), BehaviorFieldName);
        }

        public override string? GetKey()
        {
            return null;
        }

        public bool Equals(MspecBehaviorRunnerTask? other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.BehaviorFieldName == BehaviorFieldName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecBehaviorRunnerTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecBehaviorRunnerTask);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(ProjectId)
                .And(ContextTypeName)
                .And(BehaviorFieldName);
        }
    }
}
