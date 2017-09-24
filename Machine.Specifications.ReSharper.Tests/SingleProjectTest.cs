using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using Machine.Specifications.ReSharperProvider.Reflection;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    public abstract class SingleProjectTest : BaseTestWithSingleProject
    {
        public abstract ICollector Collector { get; }

        protected override string RelativeTestDataPath => "Files";

        protected IEnumerable<IFieldInfo> Fields => Collector.Fields;

        protected IEnumerable<ITypeInfo> Types => Collector.Types;

        protected ITypeInfo Type(string name = null)
        {
            var type = string.IsNullOrEmpty(name)
                ? Collector.Types.FirstOrDefault()
                : Collector.Types.FirstOrDefault(x => x.ShortName == name);

            Assert.That(type, Is.Not.Null, $"Type not found: {name}");

            return type;
        }

        protected IFieldInfo Field(string name = null)
        {
            var field = string.IsNullOrEmpty(name)
                ? Collector.Fields.FirstOrDefault()
                : Collector.Fields.FirstOrDefault(x => x.ShortName == name);

            Assert.That(field, Is.Not.Null, $"Field not found: {name}");

            return field;
        }

        public void WithFile(string filename, Action<MspecContext> action)
        {
            WithSingleProject(filename, (lifetime, solution, project) =>
            {
                Locks.ReentrancyGuard.Execute(GetType().Name, () =>
                {
                    Collector.Types.Clear();
                    Collector.Fields.Clear();

                    using (ReadLockCookie.Create())
                        WithFile(project, filename, action);
                });
            });
        }

        protected abstract void WithFile(IProject project, string filename, Action<MspecContext> action);
    }
}