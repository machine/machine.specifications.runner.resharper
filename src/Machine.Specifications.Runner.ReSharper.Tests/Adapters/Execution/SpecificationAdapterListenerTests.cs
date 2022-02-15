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
    public class SpecificationAdapterListenerTests
    {
        [Test]
        public void CanNotifyRun()
        {
            var sink = Substitute.For<IExecutionListener>();
            var cache = new ElementCache(Array.Empty<ISpecificationElement>());
            var tracker = new RunTracker(Array.Empty<ISpecificationElement>());

            var listener = new SpecificationAdapterListener(sink, cache, tracker);

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

            var listener = new SpecificationAdapterListener(sink, cache, tracker);

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
                ElementFixtures.Specification,
                ElementFixtures.BehaviorSpecification,
                ElementFixtures.SecondBehaviorSpecification
            };

            var cache = new ElementCache(elements);
            var tracker = new RunTracker(elements);

            var listener = new SpecificationAdapterListener(sink, cache, tracker);

            var context = new ContextInfo(null, null, ElementFixtures.Context.TypeName, null, null);

            listener.OnContextStart(context);
            listener.OnContextEnd(context);

            sink.Received().OnContextStart(ElementFixtures.Context);
            sink.Received().OnContextEnd(ElementFixtures.Context);
        }

        [Test]
        public void CanStartAndEndSpecification()
        {
            var sink = Substitute.For<IExecutionListener>();
            var elements = new ISpecificationElement[]
            {
                ElementFixtures.Specification,
                ElementFixtures.BehaviorSpecification,
                ElementFixtures.SecondBehaviorSpecification
            };

            var cache = new ElementCache(elements);
            var tracker = new RunTracker(elements);

            var listener = new SpecificationAdapterListener(sink, cache, tracker);

            var context = new ContextInfo(null, null, ElementFixtures.Context.TypeName, null, null);
            var specification = new SpecificationInfo(null, null, ElementFixtures.Context.TypeName, ElementFixtures.Specification.FieldName);

            listener.OnContextStart(context);
            listener.OnSpecificationStart(specification);
            listener.OnSpecificationEnd(specification, Result.Pass());
            listener.OnContextEnd(context);

            sink.Received().OnContextStart(ElementFixtures.Context);
            sink.Received().OnSpecificationStart(ElementFixtures.Specification);
            sink.Received().OnSpecificationEnd(ElementFixtures.Specification, Arg.Any<Result>());
            sink.Received().OnContextEnd(ElementFixtures.Context);
        }

        [Test]
        public void CanStartAndEndBehaviorSpecifications()
        {
            var sink = Substitute.For<IExecutionListener>();
            var elements = new ISpecificationElement[]
            {
                ElementFixtures.BehaviorSpecification,
                ElementFixtures.SecondBehaviorSpecification
            };

            var cache = new ElementCache(elements);
            var tracker = new RunTracker(elements);

            var listener = new SpecificationAdapterListener(sink, cache, tracker);

            var context = new ContextInfo(null, null, ElementFixtures.Context.TypeName, null, null);
            var specification1 = new SpecificationInfo(null, null, ElementFixtures.Behavior.TypeName, ElementFixtures.BehaviorSpecification.FieldName);
            var specification2 = new SpecificationInfo(null, null, ElementFixtures.Behavior.TypeName, ElementFixtures.SecondBehaviorSpecification.FieldName);

            listener.OnContextStart(context);
            listener.OnSpecificationStart(specification1);
            listener.OnSpecificationEnd(specification1, Result.Pass());
            listener.OnSpecificationStart(specification2);
            listener.OnSpecificationEnd(specification2, Result.Pass());
            listener.OnContextEnd(context);

            sink.Received().OnContextStart(ElementFixtures.Context);
            sink.Received().OnBehaviorStart(ElementFixtures.Behavior);
            sink.Received().OnSpecificationStart(ElementFixtures.BehaviorSpecification);
            sink.Received().OnSpecificationEnd(ElementFixtures.BehaviorSpecification, Arg.Any<Result>());
            sink.Received().OnSpecificationStart(ElementFixtures.SecondBehaviorSpecification);
            sink.Received().OnSpecificationEnd(ElementFixtures.SecondBehaviorSpecification, Arg.Any<Result>());
            sink.Received().OnBehaviorEnd(ElementFixtures.Behavior);
            sink.Received().OnContextEnd(ElementFixtures.Context);
        }
    }
}
