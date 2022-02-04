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
    public class TestRunListenerTests
    {
        [Test]
        public void CanNotifyRun()
        {
            var sink = Substitute.For<IExecutionListener>();
            var results = new ResultsContainer(Array.Empty<IMspecElement>());

            var listener = new TestRunListener(sink, results);

            listener.OnRunStart();
            listener.OnRunEnd();

            Assert.IsTrue(listener.Finished.WaitOne(1));
            sink.Received().OnRunStart();
            sink.Received().OnRunEnd();
        }

        [Test]
        public void CanNotifyAssembly()
        {
            const string path = "path/to/assembly.dll";

            var sink = Substitute.For<IExecutionListener>();
            var results = new ResultsContainer(Array.Empty<IMspecElement>());

            var listener = new TestRunListener(sink, results);

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
            var results = new ResultsContainer(new IMspecElement[]
            {
                ElementFixtures.Context,
                ElementFixtures.Behavior,
                ElementFixtures.Specification,
                ElementFixtures.BehaviorSpecification,
                ElementFixtures.SecondBehaviorSpecification
            });

            var listener = new TestRunListener(sink, results);

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
            var results = new ResultsContainer(new IMspecElement[]
            {
                ElementFixtures.Context,
                ElementFixtures.Behavior,
                ElementFixtures.Specification,
                ElementFixtures.BehaviorSpecification,
                ElementFixtures.SecondBehaviorSpecification
            });

            var listener = new TestRunListener(sink, results);

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
    }
}
