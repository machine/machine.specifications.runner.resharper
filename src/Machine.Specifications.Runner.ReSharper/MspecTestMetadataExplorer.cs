﻿using System.Linq;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.Runner.ReSharper.Reflection;

namespace Machine.Specifications.Runner.ReSharper
{
    public class MspecTestMetadataExplorer
    {
        private readonly IProject project;

        private readonly UnitTestElementFactory factory;

        private readonly IUnitTestElementsObserver observer;

        public MspecTestMetadataExplorer(IProject project, UnitTestElementFactory factory, IUnitTestElementsObserver observer)
        {
            this.project = project;
            this.factory = factory;
            this.observer = observer;
        }

        public void ExploreAssembly(IMetadataAssembly assembly, CancellationToken token)
        {
            using (ReadLockCookie.Create())
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
                project,
                new ClrTypeName(type.FullyQualifiedName),
                type.GetSubject(),
                type.GetTags().ToArray(),
                type.IsIgnored(),
                out _);

            observer.OnUnitTestElement(contextElement);

            var fields = type.GetFields().ToArray();

            var specifications = fields.Where(x => x.IsSpecification());
            var behaviors = fields.Where(x => x.IsBehavior());

            foreach (var specification in specifications)
            {
                ExploreSpecification(contextElement, type, specification);
            }

            foreach (var behavior in behaviors)
            {
                ExploreBehavior(contextElement, type, behavior);
            }

            observer.OnUnitTestElementChanged(contextElement);
        }

        private void ExploreSpecification(IUnitTestElement contextElement, ITypeInfo type, IFieldInfo field)
        {
            var specificationElement = factory.GetOrCreateContextSpecification(
                project,
                contextElement,
                new ClrTypeName(type.FullyQualifiedName),
                field.ShortName,
                field.IsIgnored());

            observer.OnUnitTestElement(specificationElement);
        }

        private void ExploreBehavior(IUnitTestElement contextElement, ITypeInfo type, IFieldInfo field)
        {
            var behaviorType = field.FieldType.GetGenericArguments()
                .FirstOrDefault();

            var behaviorElement = factory.GetOrCreateBehavior(
                project,
                contextElement,
                new ClrTypeName(type.FullyQualifiedName),
                field.ShortName,
                field.IsIgnored());

            observer.OnUnitTestElement(behaviorElement);

            if (behaviorType != null)
            {
                var behaviorSpecifications = behaviorType.GetFields()
                    .Where(x => x.IsSpecification());

                foreach (var behaviorSpecification in behaviorSpecifications)
                {
                    ExploreBehaviorSpecification(behaviorElement, type, behaviorSpecification);
                }
            }
        }

        private void ExploreBehaviorSpecification(IUnitTestElement behaviorElement, ITypeInfo type, IFieldInfo field)
        {
            var specificationElement = factory.GetOrCreateBehaviorSpecification(
                project,
                behaviorElement,
                new ClrTypeName(type.FullyQualifiedName),
                field.ShortName,
                field.IsIgnored());

            observer.OnUnitTestElement(specificationElement);
        }
    }
}
