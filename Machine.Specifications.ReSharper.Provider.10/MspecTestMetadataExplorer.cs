using System.Linq;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Reflection;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider
{
    public class MspecTestMetadataExplorer
    {
        private readonly UnitTestElementFactory _factory;
        private readonly IUnitTestElementsObserver _observer;

        public MspecTestMetadataExplorer(UnitTestElementFactory factory, IUnitTestElementsObserver observer)
        {
            _factory = factory;
            _observer = observer;
        }

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, CancellationToken token)
        {
            using (ReadLockCookie.Create())
            {
                var types = assembly.GetTypes()
                    .Flatten(x => x.GetNestedTypes());

                foreach (var type in types)
                {
                    if (token.IsCancellationRequested)
                        break;

                    ExploreType(project, assembly, type.AsTypeInfo());
                }
            }
        }

        private void ExploreType(IProject project, IMetadataAssembly assembly, ITypeInfo type)
        {
            if (!type.IsContext())
                return;

            ExploreContext(project, assembly, type);
        }

        private void ExploreContext(IProject project, IMetadataAssembly assembly, ITypeInfo type)
        {
            var contextElement = _factory.GetOrCreateContext(
                project,
                new ClrTypeName(type.FullyQualifiedName),
                assembly.Location,
                type.GetSubject(),
                type.GetTags().ToArray(),
                type.IsIgnored(),
                UnitTestElementCategorySource.Artefact,
                out _);

            _observer.OnUnitTestElement(contextElement);

            var fields = type.GetFields().ToArray();

            var specifications = fields.Where(x => x.IsSpecification());
            var behaviors = fields.Where(x => x.IsBehavior());

            foreach (var specification in specifications)
                ExploreSpecification(project, contextElement, type, specification);

            foreach (var behavior in behaviors)
                ExploreBehavior(project, contextElement, type, behavior);

            _observer.OnUnitTestElementChanged(contextElement);
        }

        private void ExploreSpecification(IProject project, IUnitTestElement contextElement, ITypeInfo type, IFieldInfo field)
        {
            var specificationElement = _factory.GetOrCreateContextSpecification(
                project,
                contextElement,
                new ClrTypeName(type.FullyQualifiedName),
                field.ShortName,
                field.IsIgnored());

            _observer.OnUnitTestElement(specificationElement);
        }

        private void ExploreBehavior(IProject project, IUnitTestElement contextElement, ITypeInfo type, IFieldInfo field)
        {
            var behaviorType = field.FieldType.GetGenericArguments()
                .FirstOrDefault();

            var behaviorElement = _factory.GetOrCreateBehavior(
                project,
                contextElement,
                new ClrTypeName(type.FullyQualifiedName),
                field.ShortName,
                field.IsIgnored());

            _observer.OnUnitTestElement(behaviorElement);

            if (behaviorType != null)
            {
                var behaviorSpecifications = behaviorType.GetFields()
                    .Where(x => x.IsSpecification());

                foreach (var behaviorSpecification in behaviorSpecifications)
                    ExploreBehaviorSpecification(project, behaviorElement, behaviorSpecification);
            }
        }

        private void ExploreBehaviorSpecification(IProject project, IUnitTestElement behaviorElement, IFieldInfo field)
        {
            var specificationElement = _factory.GetOrCreateBehaviorSpecification(
                project,
                behaviorElement,
                new ClrTypeName(field.DeclaringType),
                field.ShortName,
                field.IsIgnored());

            _observer.OnUnitTestElement(specificationElement);
        }
    }
}
