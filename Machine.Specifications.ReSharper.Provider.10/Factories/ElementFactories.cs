using JetBrains.ProjectModel;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    [SolutionComponent]
    public class ElementFactories
    {
        public ElementFactories(ContextFactory contexts,
                                ContextSpecificationFactory contextSpecifications,
                                BehaviorFactory behaviors,
                                BehaviorSpecificationFactory behaviorSpecifications)
        {
            Contexts = contexts;
            ContextSpecifications = contextSpecifications;
            Behaviors = behaviors;
            BehaviorSpecifications = behaviorSpecifications;
        }

        public ContextFactory Contexts { get; }

        public ContextSpecificationFactory ContextSpecifications { get; }

        public BehaviorFactory Behaviors { get; }

        public BehaviorSpecificationFactory BehaviorSpecifications { get; }
    }
}