using System;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider.Presentation;

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
            var element = GetOrCreateElement(typeName.FullName, x =>
                new ContextElement(x, typeName, _serviceProvider, subject, ignored));

            element.OwnCategories = _serviceProvider.CategoryFactory.Create(tags);
            element.AssemblyLocation = assemblyLocation;

            var invalidChildren = element.Children.Where(x => x.State == UnitTestElementState.Invalid);
            _serviceProvider.ElementManager.RemoveElements(invalidChildren.ToSet());

            return element;
        }

        public IUnitTestElement GetOrCreateBehavior(
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            string fieldType,
            bool ignored)
        {
            var id = $"{typeName.FullName}.{fieldName}";

            var element = GetOrCreateElement(id, x =>
                new BehaviorElement(x, parent, typeName, _serviceProvider, fieldName, ignored, fieldType));

            element.Parent = parent;
            element.OwnCategories = parent.OwnCategories;

            return element;
        }

        public IUnitTestElement GetOrCreateContextSpecification(
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            bool ignored)
        {
            var id = $"{typeName.FullName}.{fieldName}";

            var element = GetOrCreateElement(id, x =>
                new ContextSpecificationElement(x, parent, typeName, _serviceProvider, fieldName, ignored || parent.Explicit));

            element.Parent = parent;
            element.OwnCategories = parent.OwnCategories;

            return element;
        }

        public IUnitTestElement GetOrCreateBehaviorSpecification(
            IUnitTestElement parent,
            IClrTypeName typeName,
            string fieldName,
            bool isIgnored)
        {
            var id = $"{parent.Id.Id}.{typeName.FullName}.{fieldName}";

            var element = GetOrCreateElement(id, x =>
                new BehaviorSpecificationElement(x, parent, typeName, _serviceProvider, fieldName, isIgnored || parent.Explicit));

            element.Parent = parent;
            element.OwnCategories = parent.OwnCategories;

            return element;
        }

        private T GetElementById<T>(UnitTestElementId id)
            where T : Element
        {
            return _serviceProvider.ElementManager.GetElementById(id) as T;
        }

        private T GetOrCreateElement<T>(string id, Func<UnitTestElementId, T> factory)
            where T : Element
        {
            var elementId = _serviceProvider.CreateId(_project, _targetFrameworkId, id);

            return GetElementById<T>(elementId) ?? factory(elementId);
        }
    }
}
