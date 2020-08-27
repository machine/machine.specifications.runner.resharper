using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecContextTask : RemoteTask, IEquatable<MspecContextTask>
    {
        [UsedImplicitly]
        public MspecContextTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, AttributeNames.ProjectId);
            ContextTypeName = GetXmlAttribute(element, AttributeNames.ContextTypeName);
        }

        public MspecContextTask(string projectId, string contextTypeName)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
        }

        public string ProjectId { get; }

        public string ContextTypeName { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.ProjectId, ProjectId);
            SetXmlAttribute(element, AttributeNames.ContextTypeName, ContextTypeName);
        }

        public bool Equals(MspecContextTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecContextTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecContextTask);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(ProjectId)
                .And(ContextTypeName);
        }

        public string GetId()
        {
            return ContextTypeName;
        }

        public bool IsContext()
        {
            return true;
        }
    }
}
