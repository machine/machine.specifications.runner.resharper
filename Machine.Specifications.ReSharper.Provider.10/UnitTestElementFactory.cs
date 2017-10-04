using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider.Elements;

namespace Machine.Specifications.ReSharperProvider
{
    public class UnitTestElementFactory
    {
        private readonly MspecServiceProvider _serviceProvider;
        private readonly IProject _project;
        private readonly TargetFrameworkId _targetFrameworkId;

        public UnitTestElementFactory(MspecServiceProvider serviceProvider, IProject project, TargetFrameworkId targetFrameworkId)
        {
            _serviceProvider = serviceProvider;
            _project = project;
            _targetFrameworkId = targetFrameworkId;
        }

        public IUnitTestElement GetOrCreateContext(
            IClrTypeName typeName,
            FileSystemPath assemblyLocation,
            string subject,
            string[] tags,
            bool ignored)
        {
            var categories = _serviceProvider.CategoryFactory.Create(tags);

            var element = GetOrCreateElement(typeName.FullName, null, categories, x =>
                new ContextElement(x, typeName, _serviceProvider, subject, ignored));

            element.AssemblyLocation = assemblyLocation;

            return element;
        }

        public IUnitTestElement GetOrCreateBehavior(
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            bool ignored)
        {
            var id = $"{typeName.FullName}.{fieldName}";

            return GetOrCreateElement(id, parent, parent.OwnCategories, x =>
                new BehaviorElement(x, parent, typeName, _serviceProvider, fieldName, ignored));
        }

        public IUnitTestElement GetOrCreateContextSpecification(
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            bool ignored)
        {
            var id = $"{typeName.FullName}.{fieldName}";

            return GetOrCreateElement(id, parent, parent.OwnCategories, x =>
                new ContextSpecificationElement(x, parent, typeName, _serviceProvider, fieldName, ignored || parent.Explicit));
        }

        public IUnitTestElement GetOrCreateBehaviorSpecification(
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            bool isIgnored)
        {
            var id = $"{parent.Id.Id}.{typeName.FullName}.{fieldName}";

            return GetOrCreateElement(id, parent, parent.OwnCategories, x =>
                new BehaviorSpecificationElement(x, parent, typeName, _serviceProvider, fieldName, isIgnored || parent.Explicit));
        }

        private T GetElementById<T>(UnitTestElementId id)
            where T : Element
        {
            return _serviceProvider.ElementManager.GetElementById(id) as T;
        }

        private T GetOrCreateElement<T>(string id, IUnitTestElement parent, ISet<UnitTestElementCategory> categories, Func<UnitTestElementId, T> factory)
            where T : Element
        {
            var elementId = _serviceProvider.CreateId(_project, _targetFrameworkId, id);

            var element = GetElementById<T>(elementId) ?? factory(elementId);

            var invalidChildren = element.Children.Where(x => x.State == UnitTestElementState.Invalid);
            _serviceProvider.ElementManager.RemoveElements(invalidChildren.ToSet());

            element.Parent = parent;
            element.OwnCategories = categories;

            return element;
        }
    }
}
