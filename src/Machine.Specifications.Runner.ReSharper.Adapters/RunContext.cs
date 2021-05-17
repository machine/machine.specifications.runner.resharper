﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class RunContext
    {
        private readonly RemoteTaskDepot depot;

        private readonly ITestExecutionSink sink;

        private readonly ConcurrentDictionary<MspecReSharperId, TaskWrapper> contexts = new();

        private readonly ConcurrentDictionary<MspecReSharperId, TaskWrapper> specifications = new();

        private readonly ConcurrentDictionary<MspecReSharperId, TaskWrapper> behaviorSpecifications = new();

        public RunContext(RemoteTaskDepot depot, ITestExecutionSink sink)
        {
            this.depot = depot;
            this.sink = sink;
        }

        public IEnumerable<string> GetContextNames()
        {
            return depot.GetContextNames();
        }

        public TaskWrapper GetTask(ContextInfo context)
        {
            var key = new MspecReSharperId(context);

            return contexts.GetOrAdd(key, x =>
            {
                var task = depot[x] ?? CreateTask(context);

                return new TaskWrapper(task, sink);
            });
        }

        public TaskWrapper GetTask(SpecificationInfo specification)
        {
            var key = new MspecReSharperId(specification);

            return specifications.GetOrAdd(key, x =>
            {
                var task = depot[x] ?? CreateTask(specification);

                return new TaskWrapper(task, sink);
            });
        }

        public TaskWrapper GetTask(ContextInfo context, SpecificationInfo specification)
        {
            var key = new MspecReSharperId(context, specification);

            if (depot[key] == null)
            {
                return GetTask(specification);
            }

            return behaviorSpecifications.GetOrAdd(key, x =>
            {
                var task = depot[x] ?? CreateTask(context);

                return new TaskWrapper(task, sink);
            });
        }

        private RemoteTask CreateTask(object value)
        {
            return RemoteTaskBuilder.GetRemoteTask(value);
        }
    }
}
