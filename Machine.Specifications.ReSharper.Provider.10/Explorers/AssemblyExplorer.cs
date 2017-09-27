using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Elements;
using Machine.Specifications.ReSharperProvider.Factories;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    [SolutionComponent]
    public class AssemblyExplorer
    {
        private readonly ElementFactories _factories;

        public AssemblyExplorer(ElementFactories factories)
        {
            _factories = factories;
        }

        public void Explore(IProject project, IMetadataAssembly assembly, IUnitTestElementsObserver consumer, IMetadataTypeInfo metadataTypeInfo)
        {
            if (!metadataTypeInfo.IsContext())
                return;

            ContextElement contextElement = _factories.Contexts.CreateContext(consumer, project, assembly.Location, metadataTypeInfo);

            consumer.OnUnitTestElement(contextElement);

            metadataTypeInfo.GetSpecifications()
                .ToList()
                .ForEach(x =>
                {
                    var contextSpecificationElement = _factories.ContextSpecifications.CreateContextSpecification(consumer, contextElement, x);
                    consumer.OnUnitTestElement(contextSpecificationElement);
                });

            metadataTypeInfo.GetBehaviors().ToList().ForEach(x =>
            {
                var behaviorElement = _factories.Behaviors.CreateBehavior(contextElement, x, consumer);
                consumer.OnUnitTestElement(behaviorElement);

                _factories.BehaviorSpecifications
                    .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x, consumer)
                    .ToList()
                    .ForEach(consumer.OnUnitTestElement);

                consumer.OnUnitTestElementChanged(behaviorElement);
            });

            consumer.OnUnitTestElementChanged(contextElement);
        }
    }
}