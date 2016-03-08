using Machine.Specifications.ReSharperProvider.Presentation;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;

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

            ContextElement contextElement = this._factories.Contexts.CreateContext(project, assembly.Location.FullPath, metadataTypeInfo);

            consumer.OnUnitTestElement(contextElement);

            metadataTypeInfo.GetSpecifications()
                .ForEach(x =>
                {
                    var element = this._factories.ContextSpecifications.CreateContextSpecification(contextElement, x);
                    consumer.OnUnitTestElement(element);
                    consumer.OnUnitTestElementChanged(element);
                });


            metadataTypeInfo.GetBehaviors().ForEach(x =>
            {
                var behaviorElement = this._factories.Behaviors.CreateBehavior(contextElement, x);
                consumer.OnUnitTestElement(behaviorElement);
                consumer.OnUnitTestElementChanged(behaviorElement);


                this._factories.BehaviorSpecifications
                            .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x)
                            .ForEach(y =>
                            {
                                consumer.OnUnitTestElement(y);
                                consumer.OnUnitTestElementChanged(y);
                            });
            });
        }
    }
}