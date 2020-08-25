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
                case MspecTestContextRemoteTask contextTask:
                    return MspecTestContextRemoteTask.ToServer(contextTask.TypeName);

                case MspecTestSpecificationRemoteTask specificationTask:
                    return MspecTestSpecificationRemoteTask.ToServer(
                        specificationTask.ParentId,
                        specificationTask.ContextTypeName,
                        specificationTask.SpecificationFieldName,
                        specificationTask.DisplayName,
                        specificationTask.Subject,
                        specificationTask.Tags);

                default:
                    throw new ArgumentOutOfRangeException(nameof(task));
            }
        }
    }
}
