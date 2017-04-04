using Machine.Specifications.ReSharperProvider.Presentation;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    using System.Linq;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Factories;

    [SolutionComponent]
    public class AssemblyExplorer
    {
        readonly ElementFactories _factories;

        public AssemblyExplorer(ElementFactories factories)
        {
            this._factories = factories;
        }

        public void Explore(IProject project, IMetadataAssembly assembly, IUnitTestElementsObserver consumer, IMetadataTypeInfo metadataTypeInfo)
        {
            if (!metadataTypeInfo.IsContext())
            {
                return;
            }

            ContextElement contextElement = this._factories.Contexts.CreateContext(consumer, project, assembly.Location.FullPath, metadataTypeInfo);

            consumer.OnUnitTestElement(contextElement);

            metadataTypeInfo.GetSpecifications()
                .ToList()
                .ForEach(x =>
                {
                    var contextSpecificationElement = this._factories.ContextSpecifications.CreateContextSpecification(consumer, contextElement, x);
                    consumer.OnUnitTestElement(contextSpecificationElement);
                });

            metadataTypeInfo.GetBehaviors().ToList().ForEach(x =>
            {
                var behaviorElement = this._factories.Behaviors.CreateBehavior(contextElement, x, consumer);
                consumer.OnUnitTestElement(behaviorElement);

                this._factories.BehaviorSpecifications
                            .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x, consumer)
                            .ToList()
                            .ForEach(consumer.OnUnitTestElement);

                consumer.OnUnitTestElementChanged(behaviorElement);
            });

            consumer.OnUnitTestElementChanged(contextElement);
        }
    }
}