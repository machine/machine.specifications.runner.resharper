using System;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using Machine.Specifications.Runner.ReSharper.Tests.Fixtures;
using Machine.Specifications.Runner.Utility;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Execution
{
    [TestFixture]
    public class ExecutionAdapterRunListenerTests
    {
        [Test]
        public void CanNotifyRun()
        {
            var sink = Substitute.For<IExecutionListener>();
            var cache = new ElementCache(Array.Empty<ISpecificationElement>());
            var tracker = new RunTracker(Array.Empty<ISpecificationElement>());

            var listener = new ExecutionAdapterRunListener(sink, cache, tracker);

            listener.OnRunStart();
            listener.OnRunEnd();

            sink.Received().OnRunStart();
            sink.Received().OnRunEnd();
        }

        [Test]
        public void CanNotifyAssembly()
        {
            const string path = "path/to/assembly.dll";

            var sink = Substitute.For<IExecutionListener>();
            var cache = new ElementCache(Array.Empty<ISpecificationElement>());
            var tracker = new RunTracker(Array.Empty<ISpecificationElement>());

            var listener = new ExecutionAdapterRunListener(sink, cache, tracker);

            var assembly = new AssemblyInfo("Assembly", path);

            listener.OnAssemblyStart(assembly);
            listener.OnAssemblyEnd(assembly);

            sink.Received().OnAssemblyStart(path);
            sink.Received().OnAssemblyEnd(path);
        }

        [Test]
        public void CanStartAndEndContext()
        {
            var sink = Substitute.For<IExecutionListener>();
            var elements = new ISpecificationElement[]
            {
                ElementFixtures.Specification1,
                ElementFixtures.Behavior1Specification1,
                ElementFixtures.Behavior1Specification2
            };

            var cache = new ElementCache(elements);
            var tracker = new RunTracker(elements);

            var listener = new ExecutionAdapterRunListener(sink, cache, tracker);

            var context = new ContextInfo(null, null, ElementFixtures.Context.TypeName, null, null);

            listener.OnContextStart(context);
            listener.OnContextEnd(context);

            sink.Received().OnContextStart(ElementFixtures.Context);
            sink.Received().OnContextEnd(ElementFixtures.Context, Arg.Any<string>());
        }

        [Test]
        public void CanStartAndEndSpecification()
        {
            var sink = Substitute.For<IExecutionListener>();
            var elements = new ISpecificationElement[]
            {
                ElementFixtures.Specification1,
                ElementFixtures.Behavior1Specification1,
                ElementFixtures.Behavior1Specification2
            };

            var cache = new ElementCache(elements);
            var tracker = new RunTracker(elements);

            var listener = new ExecutionAdapterRunListener(sink, cache, tracker);

            var context = new ContextInfo(null, null, ElementFixtures.Context.TypeName, null, null);
            var specification = new SpecificationInfo(null, null, ElementFixtures.Context.TypeName, ElementFixtures.Specification1.FieldName);

            listener.OnContextStart(context);
            listener.OnSpecificationStart(specification);
            listener.OnSpecificationEnd(specification, Result.Pass());
            listener.OnContextEnd(context);

            sink.Received().OnContextStart(ElementFixtures.Context);
            sink.Received().OnSpecificationStart(ElementFixtures.Specification1);
            sink.Received().OnSpecificationEnd(ElementFixtures.Specification1, Arg.Any<string>(), Arg.Any<Result>());
            sink.Received().OnContextEnd(ElementFixtures.Context, Arg.Any<string>());
        }

        [Test]
        public void CanStartAndEndBehaviorSpecifications()
        {
            var sink = Substitute.For<IExecutionListener>();
            var elements = new ISpecificationElement[]
            {
                ElementFixtures.Behavior1Specification1,
                ElementFixtures.Behavior1Specification2
            };

            var cache = new ElementCache(elements);
            var tracker = new RunTracker(elements);

            var listener = new ExecutionAdapterRunListener(sink, cache, tracker);

            var context = new ContextInfo(null, null, ElementFixtures.Context.TypeName, null, null);
            var specification1 = new SpecificationInfo(null, null, ElementFixtures.Behavior1.TypeName, ElementFixtures.Behavior1Specification1.FieldName);
            var specification2 = new SpecificationInfo(null, null, ElementFixtures.Behavior1.TypeName, ElementFixtures.Behavior1Specification2.FieldName);

            listener.OnContextStart(context);
            listener.OnSpecificationStart(specification1);
            listener.OnSpecificationEnd(specification1, Result.Pass());
            listener.OnSpecificationStart(specification2);
            listener.OnSpecificationEnd(specification2, Result.Pass());
            listener.OnContextEnd(context);

            sink.Received().OnContextStart(ElementFixtures.Context);
            sink.Received().OnBehaviorStart(ElementFixtures.Behavior1);
            sink.Received().OnSpecificationStart(ElementFixtures.Behavior1Specification1);
            sink.Received().OnSpecificationEnd(ElementFixtures.Behavior1Specification1, Arg.Any<string>(), Arg.Any<Result>());
            sink.Received().OnSpecificationStart(ElementFixtures.Behavior1Specification2);
            sink.Received().OnSpecificationEnd(ElementFixtures.Behavior1Specification2, Arg.Any<string>(), Arg.Any<Result>());
            sink.Received().OnBehaviorEnd(ElementFixtures.Behavior1, Arg.Any<string>());
            sink.Received().OnContextEnd(ElementFixtures.Context, Arg.Any<string>());
        }
    }
}
