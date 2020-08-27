using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class RemoteTaskBuilder
    {
        public static RemoteTask GetRemoteTask(RemoteTask task)
        {
            switch (task)
            {
                case MspecContextRemoteTask contextTask:
                    return MspecContextRemoteTask.ToServer(
                        contextTask.ContextTypeName,
                        contextTask.Subject,
                        contextTask.Tags);

                case MspecSpecificationRemoteTask specificationTask:
                    return MspecSpecificationRemoteTask.ToServer(
                        specificationTask.ContextTypeName,
                        specificationTask.SpecificationFieldName,
                        specificationTask.DisplayName,
                        specificationTask.Subject,
                        specificationTask.Tags);

                case MspecBehaviorRemoteTask behaviorTask:
                    return MspecBehaviorRemoteTask.ToServer(
                        behaviorTask.ContextTypeName,
                        behaviorTask.BehaviorFieldName,
                        behaviorTask.SpecificationFieldName);

                default:
                    throw new ArgumentOutOfRangeException(nameof(task));
            }
        }
    }
}
