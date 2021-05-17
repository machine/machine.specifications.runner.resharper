using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public abstract class MspecRunnerTask : RemoteTask
    {
        protected MspecRunnerTask(string ignoreReason)
            : base(MspecTaskRunner.RunnerId)
        {
            IgnoreReason = ignoreReason;
        }

        protected MspecRunnerTask(XmlElement element)
            : base(element)
        {
            IgnoreReason = GetXmlAttribute(element, nameof(IgnoreReason));
        }

        public string IgnoreReason { get; }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, nameof(IgnoreReason), IgnoreReason);
        }

        public abstract string GetKey();

        public MspecRunnerTask AsRemoteTask()
        {
            return this;
        }
    }
}
