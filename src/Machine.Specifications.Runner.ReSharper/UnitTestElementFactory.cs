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
        private readonly MspecServiceProvider serviceProvider;

        private readonly TargetFrameworkId targetFrameworkId;

        private readonly Action<IUnitTestElement> elementChangedAction;

        private readonly UnitTestElementOrigin origin;

        private readonly Dictionary<UnitTestElementId, IUnitTestElement> elements = new Dictionary<UnitTestElementId, IUnitTestElement>();

        public UnitTestElementFactory(
            MspecServiceProvider serviceProvider,
            TargetFrameworkId targetFrameworkId,
            Action<IUnitTestElement> elementChangedAction,
            UnitTestElementOrigin origin)
        {
            this.serviceProvider = serviceProvider;
            this.targetFrameworkId = targetFrameworkId;
            this.elementChangedAction = elementChangedAction;
            this.origin = origin;
        }

        public ContextElement GetOrCreateContext(
            IProject project,
            IClrTypeName typeName,
            string subject,
            string[] tags,
            bool ignored,
            out bool tagsChanged)
        {
            lock (elements)
            {
                var element = GetOrCreateElement(typeName.FullName, project, null, null, x =>
                    new ContextElement(x, typeName, serviceProvider, subject, ignored));

                tagsChanged = UpdateCategories(element, tags);

                return element;
            }
        }

        public IUnitTestElement GetOrCreateBehavior(
            IProject project,
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            bool ignored)
        {
            lock (elements)
            {
                var id = $"{typeName.FullName}::{fieldName}";

                return GetOrCreateElement(id, project, parent, parent.OwnCategories, x =>
                    new BehaviorElement(x, parent, typeName, serviceProvider, fieldName, ignored));
            }
        }

        public ContextSpecificationElement GetOrCreateContextSpecification(
            IProject project,
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            bool ignored)
        {
            lock (elements)
            {
                var id = $"{typeName.FullName}::{fieldName}";

                return GetOrCreateElement(id, project, parent, parent.OwnCategories, x =>
                    new ContextSpecificationElement(x, parent, typeName, serviceProvider, fieldName, ignored || parent.Explicit));
            }
        }

        public IUnitTestElement GetOrCreateBehaviorSpecification(
            IProject project,
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            bool isIgnored)
        {
            lock (elements)
            {
                var id = $"{typeName.FullName}::{fieldName}";

                return GetOrCreateElement(id, project, parent, parent.OwnCategories, x =>
                    new BehaviorSpecificationElement(x, parent, typeName, serviceProvider, fieldName, isIgnored || parent.Explicit));
            }
        }

        private T GetElementById<T>(UnitTestElementId id)
            where T : Element
        {
            if (elements.TryGetValue(id, out var element))
            {
                return element as T;
            }

            return serviceProvider.ElementManager.GetElementById<T>(id);
        }

        private T GetOrCreateElement<T>(string id, IProject project, IUnitTestElement parent, ISet<UnitTestElementCategory> categories, Func<UnitTestElementId, T> factory)
            where T : Element
        {
            var elementId = serviceProvider.CreateId(project, targetFrameworkId, id);

            var element = GetElementById<T>(elementId) ?? factory(elementId);

            element.Parent = parent;
            element.OwnCategories = categories;

            elements[elementId] = element;

            return element;
        }

        private bool UpdateCategories(Element element, string[] categories)
        {
            using (UT.WriteLock())
            {
                var result = element.UpdateOwnCategoriesFrom(categories, origin);

                if (result)
                {
                    elementChangedAction?.Invoke(element);
                }

                return result;
            }
        }
    }
}
