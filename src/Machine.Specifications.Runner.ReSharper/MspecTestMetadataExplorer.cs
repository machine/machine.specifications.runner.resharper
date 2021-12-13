using System.Linq;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Reflection;

namespace Machine.Specifications.Runner.ReSharper
{
    public class MspecTestMetadataExplorer
    {
        private readonly IUnitTestElementObserver observer;

        private readonly UnitTestElementFactory factory = new();

        public MspecTestMetadataExplorer(IUnitTestElementObserver observer)
        {
            this.observer = observer;
        }

        public void ExploreAssembly(IMetadataAssembly assembly, CancellationToken token)
        {
            var types = assembly.GetTypes()
                .Flatten(x => x.GetNestedTypes());

            foreach (var type in types)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                ExploreType(type.AsTypeInfo());
            }
        }

        private void ExploreType(ITypeInfo type)
        {
            if (!type.IsContext())
            {
                return;
            }

            ExploreContext(type);
        }

        private void ExploreContext(ITypeInfo type)
        {
            var contextElement = factory.GetOrCreateContext(
                new ClrTypeName(type.FullyQualifiedName),
                type.GetSubject(),
                type.GetTags().ToArray(),
                type.GetIgnoreReason());

            observer.OnUnitTestElement(contextElement);

            var fields = type.GetFields().ToArray();

            var specifications = fields.Where(x => x.IsSpecification());
            var behaviors = fields.Where(x => x.IsBehavior());

            foreach (var specification in specifications)
            {
                ExploreSpecification(contextElement, specification);
            }

            foreach (var behavior in behaviors)
            {
                ExploreBehavior(contextElement, behavior);
            }

            observer.OnUnitTestElement(contextElement);
        }

        private void ExploreSpecification(MspecContextTestElement contextElement, IFieldInfo field)
        {
            var specificationElement = factory.GetOrCreateContextSpecification(
                contextElement,
                field.ShortName,
                null,
                field.GetIgnoreReason());

            observer.OnUnitTestElement(specificationElement);
        }

        private void ExploreBehavior(MspecContextTestElement contextElement, IFieldInfo field)
        {
            var behaviorType = field.FieldType.GetGenericArguments()
                .FirstOrDefault();

            var specificationElement = factory.GetOrCreateContextSpecification(
                contextElement,
                field.ShortName,
                behaviorType.FullyQualifiedName,
                field.GetIgnoreReason());

            observer.OnUnitTestElement(specificationElement);
        }
    }
}
