using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.UI.BindableLinq.Collections;
using JetBrains.Application.UI.BindableLinq.Interfaces;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public abstract class MspecTestElement : IUnitTestElement
    {
        private IUnitTestElement parent;

        private string shortName;

        private IClrTypeName typeName;

        protected MspecTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string explicitReason)
        {
            Services = services;
            Id = id;
            TypeName = typeName;
            ExplicitReason = explicitReason;
        }

        public abstract string Kind { get; }

        public ISet<UnitTestElementCategory> OwnCategories { get; set; }

        public string ExplicitReason { get; }

        public UnitTestElementId Id { get; }

        public MspecServiceProvider Services { get; }

        public IUnitTestElement Parent
        {
            get => parent;
            set
            {
                if (Equals(parent, value) || Equals(this, value))
                {
                    return;
                }

                using (UT.WriteLock())
                {
                    parent?.Children.Remove(this);
                    parent = value;
                    parent?.Children.Add(this);
                }

                Services.ElementManager.FireElementChanged(this);
            }
        }

        public IBindableCollection<IUnitTestElement> Children { get; } = new BindableSetCollectionWithoutIndexTracking<IUnitTestElement>(UT.Locks.ReadLock, UnitTestElement.EqualityComparer);

        public string ShortName
        {
            get => shortName;
            protected set
            {
                if (value == shortName)
                {
                    return;
                }

                shortName = value;
                Services.ElementManager.FireElementChanged(this);
            }
        }

        public bool Explicit => !string.IsNullOrEmpty(ExplicitReason);

        public UnitTestElementOrigin Origin { get; set; }

        public IClrTypeName TypeName
        {
            get => typeName;
            private set
            {
                if (Equals(value, typeName))
                {
                    return;
                }

                typeName = value;
                Services.ElementManager.FireElementChanged(this);
            }
        }

        public UnitTestElementNamespace GetNamespace()
        {
            return UnitTestElementNamespaceFactory.Create(TypeName.NamespaceNames);
        }

        public virtual UnitTestElementDisposition GetDisposition()
        {
            var element = GetDeclaredElement();

            if (element == null || !element.IsValid())
            {
                return UnitTestElementDisposition.InvalidDisposition;
            }

            var locations = GetLocations(element);

            return new UnitTestElementDisposition(locations, this);
        }

        protected abstract string GetPresentation();

        public string GetPresentation(IUnitTestElement unitTestElement, bool full)
        {
            return full
                ? Id.Id
                : GetPresentation();
        }

        public virtual IDeclaredElement GetDeclaredElement()
        {
            return Services.CachingService.GetTypeElement(Id.Project, Id.TargetFrameworkId, TypeName, true, true);
        }

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            var declaredElement = GetDeclaredElement();

            if (declaredElement == null)
            {
                return EmptyList<IProjectFile>.InstanceList;
            }

            return declaredElement
                .GetSourceFiles()
                .SelectNotNull(x => x.ToProjectFile())
                .ToArray();
        }

        public virtual IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            return EmptyList<UnitTestTask>.Instance;
        }

        public IUnitTestRunStrategy GetRunStrategy(IHostProvider hostProvider)
        {
            return Services.GetRunStrategy(this);
        }

        protected ITypeElement GetDeclaredType()
        {
            return Services.CachingService.GetTypeElement(Id.Project, Id.TargetFrameworkId, TypeName, true, true);
        }

        public virtual IEnumerable<UnitTestElementLocation> GetLocations(IDeclaredElement element)
        {
            return element.GetDeclarations()
                .Select(x => new {declaration = x, file = x.GetContainingFile()})
                .Where(x => x.file != null)
                .Select(x => new UnitTestElementLocation(
                    x.file.GetSourceFile().ToProjectFile(),
                    x.declaration.GetNameDocumentRange().TextRange,
                    x.declaration.GetDocumentRange().TextRange));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IUnitTestElement);
        }

        public bool Equals(IUnitTestElement other)
        {
            return other != null && other.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} - {Id}";
        }
    }
}
