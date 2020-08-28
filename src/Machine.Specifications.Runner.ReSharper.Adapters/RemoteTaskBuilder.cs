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

                case MspecContextSpecificationRemoteTask specificationTask:
                    return MspecContextSpecificationRemoteTask.ToServer(
                        specificationTask.ContextTypeName,
                        specificationTask.SpecificationFieldName,
                        specificationTask.DisplayName,
                        specificationTask.Subject,
                        specificationTask.Tags);

                case MspecBehaviorRemoteTask behaviorTask:
                    return MspecBehaviorRemoteTask.ToServer(
                        behaviorTask.ContextTypeName,
                        behaviorTask.BehaviorFieldName);

                case MspecBehaviorSpecificationRemoteTask behaviorSpecificationTask:
                    return MspecBehaviorSpecificationRemoteTask.ToServer(
                        behaviorSpecificationTask.ContextTypeName,
                        behaviorSpecificationTask.BehaviorFieldName,
                        behaviorSpecificationTask.SpecificationFieldName);

                default:
                    throw new ArgumentOutOfRangeException(nameof(task));
            }
        }
    }
}
