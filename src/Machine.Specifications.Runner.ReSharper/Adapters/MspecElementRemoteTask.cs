﻿using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    [Serializable]
    public abstract class MspecElementRemoteTask : RemoteTask
    {
        protected MspecElementRemoteTask(string testId, bool runAllChildren = true, bool runExplicitly = false)
        {
            TestId = testId;
            RunAllChildren = runAllChildren;
            RunExplicitly = runExplicitly;
        }

        public string TestId { get; }

        public bool RunAllChildren { get; }

        public bool RunExplicitly { get; }

        public override string ToString()
        {
            return GetType().Name + "(" + TestId + ")";
        }
    }
}
