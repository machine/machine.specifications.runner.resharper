﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class RemoteTaskDepot
    {
        private readonly Dictionary<string, MspecRemoteTask> tasksByReSharperId = new();

        private readonly HashSet<string> contextsToRun = new();

        private readonly List<Specification> testsToRun = new();

        public RemoteTaskDepot(RemoteTask[] tasks)
        {
            foreach (var task in tasks.OfType<MspecRemoteTask>())
            {
                Add(task);
            }
        }

        public MspecRemoteTask? this[string id]
        {
            get
            {
                tasksByReSharperId.TryGetValue(id, out var value);

                return value;
            }
        }

        public MspecRemoteTask? this[MspecReSharperId id]
        {
            get
            {
                tasksByReSharperId.TryGetValue(id.Id, out var value);

                return value;
            }
        }

        public MspecRemoteTask? this[TestElement element]
        {
            get
            {
                var id = MspecReSharperId.Self(element);

                tasksByReSharperId.TryGetValue(id, out var value);

                return value;
            }
        }

        public void Add(MspecRemoteTask task)
        {
            tasksByReSharperId[task.TestId] = task;

            switch (task)
            {
                case MspecContextRemoteTask context:
                    AddContext(context.ContextTypeName);
                    break;

                case MspecContextSpecificationRemoteTask specification:
                    AddContext(specification.ContextTypeName);
                    break;

                case MspecBehaviorSpecificationRemoteTask behaviorSpecification:
                    AddContext(behaviorSpecification.ContextTypeName);
                    break;
            }
        }

        public void Bind(TestElement element, MspecRemoteTask task)
        {
            if (tasksByReSharperId.ContainsKey(task.TestId) && element is Specification specification)
            {
                testsToRun.Add(specification);
            }
        }

        public IEnumerable<string> GetContextsToRun()
        {
            return contextsToRun;
        }

        public IEnumerable<RemoteTask> GetTasks()
        {
            return tasksByReSharperId.Values;
        }

        private void AddContext(string? contextName)
        {
            if (!string.IsNullOrEmpty(contextName))
            {
                contextsToRun.Add(contextName!);
            }
        }
    }
}
