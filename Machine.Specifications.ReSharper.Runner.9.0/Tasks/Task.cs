using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public abstract class Task : RemoteTask, IEquatable<Task>
    {
        protected Task(XmlElement element)
            : base(element)
        {
            AssemblyLocation = GetXmlAttribute(element, AttributeNames.AssemblyLocation);
        }

        protected Task(string providerId, string assemblyLocation)
            : base(providerId)
        {
            AssemblyLocation = assemblyLocation;
        }


        public string AssemblyLocation { get; private set; }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.AssemblyLocation, AssemblyLocation);
        }

        public bool Equals(Task other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(RunnerID, other.RunnerID) &&
                   Equals(AssemblyLocation, other.AssemblyLocation);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as Task);
        }

        public override bool Equals(object other)
        {
            return Equals(other as Task);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = Id.GetHashCode();
                result = (result * 397) ^ AssemblyLocation.GetHashCode();
                return result;
            }
        }
    }
}