using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Machine.Specifications.Runner.ReSharper.Elements;

namespace Machine.Specifications.Runner.ReSharper
{
    public class UnitTestElementFactory
    {
        private readonly MspecServiceProvider services;

        private readonly TargetFrameworkId targetFrameworkId;

        private readonly Action<IUnitTestElement>? elementChangedAction;

        private readonly UnitTestElementOrigin origin;

        private readonly Dictionary<UnitTestElementId, IUnitTestElement> elements = new();

        public UnitTestElementFactory(
            MspecServiceProvider services,
            TargetFrameworkId targetFrameworkId,
            Action<IUnitTestElement>? elementChangedAction,
            UnitTestElementOrigin origin)
        {
            this.services = services;
            this.targetFrameworkId = targetFrameworkId;
            this.elementChangedAction = elementChangedAction;
            this.origin = origin;
        }

        public MspecContextTestElement GetOrCreateContext(
            IProject project,
            IClrTypeName typeName,
            string? subject,
            string[]? tags,
            string? ignoreReason)
        {
            lock (elements)
            {
                var element = GetOrCreateElement(typeName.FullName, project, null, null, x =>
                    new MspecContextTestElement(services, x, typeName, subject, ignoreReason));

                UpdateCategories(element, tags);

                return element;
            }
        }

        public MspecBehaviorTestElement GetOrCreateBehavior(
            IProject project,
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            string? ignoreReason)
        {
            lock (elements)
            {
                var id = $"{typeName.FullName}::{fieldName}";

                return GetOrCreateElement(id, project, parent, parent.OwnCategories, x =>
                    new MspecBehaviorTestElement(services, x, typeName, fieldName, ignoreReason));
            }
        }

        public MspecContextSpecificationTestElement GetOrCreateContextSpecification(
            IProject project,
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            string? ignoreReason)
        {
            lock (elements)
            {
                var id = $"{typeName.FullName}::{fieldName}";

                return GetOrCreateElement(id, project, parent, parent.OwnCategories, x =>
                    new MspecContextSpecificationTestElement(services, x, typeName, fieldName, ignoreReason));
            }
        }

        public MspecBehaviorSpecificationTestElement GetOrCreateBehaviorSpecification(
            IProject project,
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            string? ignoreReason)
        {
            lock (elements)
            {
                var id = $"{typeName.FullName}::{fieldName}";

                return GetOrCreateElement(id, project, parent, parent.OwnCategories, x =>
                    new MspecBehaviorSpecificationTestElement(services, x, typeName, fieldName, ignoreReason));
            }
        }

        private T? GetElementById<T>(UnitTestElementId id)
            where T : MspecTestElement
        {
            if (elements.TryGetValue(id, out var element))
            {
                return element as T;
            }

            return services.ElementManager.GetElementById<T>(id);
        }

        private T GetOrCreateElement<T>(string id, IProject project, IUnitTestElement? parent, ISet<UnitTestElementCategory>? categories, Func<UnitTestElementId, T> factory)
            where T : MspecTestElement
        {
            var elementId = services.CreateId(project, targetFrameworkId, id);

            var element = GetElementById<T>(elementId) ?? factory(elementId);

            element.Parent = parent;
            element.OwnCategories = categories;

            elements[elementId] = element;

            return element;
        }

        private void UpdateCategories(MspecTestElement element, string[]? categories)
        {
            if (UpdateOwnCategories(element, categories))
            {
                if (elementChangedAction != null)
                {
                    elementChangedAction(element);
                }
                else
                {
                    element.Services.ElementManager.FireElementChanged(element);
                }
            }
        }

        private bool UpdateOwnCategories(MspecTestElement element, string[]? categories)
        {
            using (UT.WriteLock())
            {
                return element.UpdateOwnCategoriesFrom(categories, origin);
            }
        }
    }
}
