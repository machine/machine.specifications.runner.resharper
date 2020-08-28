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
    public abstract class Element : IUnitTestElement
    {
        private IUnitTestElement parent;

        protected Element(
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            bool isIgnored)
        {
            Id = id;
            TypeName = typeName;
            ServiceProvider = serviceProvider;
            Parent = parent;
            ExplicitReason = isIgnored ? "Ignored" : string.Empty;
        }

        public abstract string Kind { get; }

        public ISet<UnitTestElementCategory> OwnCategories { get; set; }

        public string ExplicitReason { get; }

        public UnitTestElementId Id { get; }

        public MspecServiceProvider ServiceProvider { get; }

        public IUnitTestElement Parent
        {
            get => parent;
            set
            {
                if (Equals(parent, value))
                {
                    return;
                }

                var oldParent = parent;

                using (UT.WriteLock())
                {
                    parent?.Children.Remove(this);
                    parent = value;
                    parent?.Children.Add(this);
                }

                ServiceProvider.ElementManager.FireElementChanged(oldParent);
                ServiceProvider.ElementManager.FireElementChanged(value);
            }
        }

        public IBindableCollection<IUnitTestElement> Children { get; } = new BindableSetCollectionWithoutIndexTracking<IUnitTestElement>(UT.Locks.ReadLock, UnitTestElement.EqualityComparer);

        public abstract string ShortName { get; }

        public bool Explicit => !string.IsNullOrEmpty(ExplicitReason);

        public UnitTestElementOrigin Origin { get; set; }

        public IClrTypeName TypeName { get; }

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

        public abstract IDeclaredElement GetDeclaredElement();

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            return GetDeclaredElement()?
                .GetSourceFiles()
                .SelectNotNull(x => x.ToProjectFile())
                .ToList();
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
            return ServiceProvider.CachingService.GetTypeElement(Id.Project, Id.TargetFrameworkId, TypeName, true, true);
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
