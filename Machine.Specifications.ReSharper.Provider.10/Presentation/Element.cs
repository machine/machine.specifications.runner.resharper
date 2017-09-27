using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Application.UI.BindableLinq.Collections;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public abstract class Element : IUnitTestElement
    {
        private IUnitTestElement _parent;

        protected Element(
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            bool isIgnored)
        {
            Id = id;
            Parent = parent;
            TypeName = typeName;
            ServiceProvider = serviceProvider;
            ExplicitReason = isIgnored ? "Ignored" : string.Empty;
        }

        public abstract string Kind { get; }

        public ISet<UnitTestElementCategory> OwnCategories { get; set; } = JetHashSet<UnitTestElementCategory>.Empty;

        public string ExplicitReason { get; }

        public UnitTestElementId Id { get; }
        
        public IUnitTestElement Parent
        {
            get => _parent;
            set
            {
                if (Equals(_parent, value))
                    return;

                var oldParent = _parent;

                using (UT.WriteLock())
                {
                    _parent?.Children.Remove(this);
                    _parent = value;
                    _parent?.Children.Add(this);
                }

                ServiceProvider.ElementManager.FireElementChanged(oldParent);
                ServiceProvider.ElementManager.FireElementChanged(value);
            }
        }

        public ICollection<IUnitTestElement> Children { get; } = new BindableCollection<IUnitTestElement>(UT.Locks.ReadLock);

        public MspecServiceProvider ServiceProvider { get; }

        public abstract string ShortName { get; }

        public bool Explicit => !string.IsNullOrEmpty(ExplicitReason);

        public UnitTestElementState State { get; set; }

        public IClrTypeName TypeName { get; }

        public UnitTestElementNamespace GetNamespace()
        {
            return UnitTestElementNamespaceFactory.Create(TypeName.NamespaceNames);
        }

        public virtual UnitTestElementDisposition GetDisposition()
        {
            var element = GetDeclaredElement();

            if (element == null || !element.IsValid())
                return UnitTestElementDisposition.InvalidDisposition;

            var locations = GetLocations(element);

            return new UnitTestElementDisposition(locations, this);
        }

        protected abstract string GetPresentation();

        public string GetPresentation(IUnitTestElement unitTestElement, bool full)
        {
            if (full)
                return Id.Id;

            return GetPresentation();
        }

        public abstract IDeclaredElement GetDeclaredElement();

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            return GetDeclaredType()?
                .GetSourceFiles()
                .Select(x => x.ToProjectFile());
        }

        public virtual IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            return EmptyArray<UnitTestTask>.Instance;
        }

        public IUnitTestRunStrategy GetRunStrategy(IHostProvider hostProvider)
        {
            return ServiceProvider.GetRunStrategy(this);
        }

        protected ITypeElement GetDeclaredType()
        {
            return ServiceProvider.CachingService.GetTypeElement(Id.Project, TargetFrameworkId.Default, TypeName, true, true);
        }

        private bool Equals(IUnitTestElement other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other.GetType() == GetType())
            {
                var element = (Element)other;
                string thisFullName;
                string otherFullName;

                try
                {
                    // This might throw for invalid elements.
                    thisFullName = TypeName.FullName;
                    otherFullName = element.TypeName.FullName;
                }
                catch (NullReferenceException)
                {
                    Debug.Fail("Should not happen as the type name should be persisted!");
                    return false;
                }

                return Equals(other.Id, Id)
                       && other.ShortName == ShortName
                       && thisFullName == otherFullName;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return Equals(obj as IUnitTestElement);
        }

        public override int GetHashCode()
        {
            var result = 0;
            result = 29 * result + Id.GetHashCode();
            result = 29 * result + ShortName.GetHashCode();
            result = 29 * result + TypeName.FullName.GetHashCode();

            return result;
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
    }
}