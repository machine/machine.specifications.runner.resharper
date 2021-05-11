using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecBehaviorSpecificationRunnerTask : RemoteTask, IEquatable<MspecBehaviorSpecificationRunnerTask>, IKeyedTask
    {
        public MspecBehaviorSpecificationRunnerTask(string projectId, string contextTypeName, string behaviorFieldName, string behaviorSpecificationFieldName)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
            BehaviorFieldName = behaviorFieldName;
            BehaviorSpecificationFieldName = behaviorSpecificationFieldName;
        }

        public MspecBehaviorSpecificationRunnerTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
            ContextTypeName = GetXmlAttribute(element, nameof(ContextTypeName));
            BehaviorFieldName = GetXmlAttribute(element, nameof(BehaviorFieldName));
            BehaviorSpecificationFieldName = GetXmlAttribute(element, nameof(BehaviorSpecificationFieldName));
        }

        public string ProjectId { get; }

        public string ContextTypeName { get; }

        public string BehaviorFieldName { get; }

        public string BehaviorSpecificationFieldName { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, nameof(ProjectId), ProjectId);
            SetXmlAttribute(element, nameof(ContextTypeName), ContextTypeName);
            SetXmlAttribute(element, nameof(BehaviorFieldName), BehaviorFieldName);
            SetXmlAttribute(element, nameof(BehaviorSpecificationFieldName), BehaviorSpecificationFieldName);
        }

        public bool Equals(MspecBehaviorSpecificationRunnerTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.BehaviorFieldName == BehaviorFieldName &&
                   other.BehaviorSpecificationFieldName == BehaviorSpecificationFieldName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecBehaviorSpecificationRunnerTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecBehaviorSpecificationRunnerTask);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(ProjectId)
                .And(ContextTypeName)
                .And(BehaviorFieldName)
                .And(BehaviorSpecificationFieldName);
        }

        public string GetKey()
        {
            return $"{ContextTypeName}.{BehaviorSpecificationFieldName}";
        }

        public RemoteTask AsRemoteTask()
        {
            return this;
        }
    }
}
