using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Application.UI.BindableLinq.Collections;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider.Factories;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public abstract class Element : IUnitTestElement
    {
        private static readonly IUnitTestRunStrategy RunStrategy = new OutOfProcessUnitTestRunStrategy(new RemoteTaskRunnerInfo(MspecTaskRunner.RunnerId, typeof(MspecTaskRunner)));

        private readonly IClrTypeName _declaringTypeName;
        private readonly UnitTestTaskFactory _taskFactory;
        private readonly MspecServiceProvider _serviceProvider;
        private IUnitTestElement _parent;

        protected Element(
            IUnitTestElement parent,
            IClrTypeName declaringTypeName,
            MspecServiceProvider serviceProvider,
            bool isIgnored)
        {
            _declaringTypeName = declaringTypeName;
            _serviceProvider = serviceProvider;

            Parent = parent;
            ExplicitReason = isIgnored ? "Ignored" : string.Empty;

            _taskFactory = new UnitTestTaskFactory();
        }

        public abstract string Kind { get; }

        public ISet<UnitTestElementCategory> OwnCategories { get; set; } = JetHashSet<UnitTestElementCategory>.Empty;

        public string ExplicitReason { get; }

        public abstract UnitTestElementId Id
        {
            get;
        }
        
        public IUnitTestElement Parent
        {
            get => _parent;
            set
            {
                if (Equals(_parent, value))
                    return;

                var oldParent = _parent;
                var newParent = value;

                using (UT.WriteLock())
                {
                    _parent?.Children.Remove(this);

                    _parent = newParent;

                    _parent?.Children.Add(this);
                }

                _serviceProvider.ElementManager.FireElementChanged(oldParent);
                _serviceProvider.ElementManager.FireElementChanged(newParent);
            }
        }

        public ICollection<IUnitTestElement> Children { get; } = new BindableCollection<IUnitTestElement>(UT.Locks.ReadLock);

        public abstract string ShortName { get; }

        public bool Explicit => false;

        public UnitTestElementState State { get; set; }

        public IProject GetProject()
        {
            return Id.Project;
        }

        public UnitTestElementNamespace GetNamespace()
        {
            return UnitTestElementNamespaceFactory.Create(_declaringTypeName.NamespaceNames);
        }

        public virtual UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = GetDeclaredElement();

            if (element == null || !element.IsValid())
                return UnitTestElementDisposition.InvalidDisposition;

            IEnumerable<UnitTestElementLocation> locations = GetLocations(element);

            return new UnitTestElementDisposition(locations, this);
        }

        protected abstract string GetPresentation();

        public string GetPresentation(IUnitTestElement unitTestElement, bool full)
        {
            return GetPresentation();
        }

        public abstract IDeclaredElement GetDeclaredElement();

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            ITypeElement declaredType = GetDeclaredType();

            if (declaredType == null)
                return EmptyArray<IProjectFile>.Instance;

            return declaredType.GetSourceFiles().Select(x => x.ToProjectFile());
        }

        public IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            if (this is ContextSpecificationElement)
            {
                var contextSpecification = this as ContextSpecificationElement;
                var context = contextSpecification.Context;

                return new List<UnitTestTask>
                {
                    _taskFactory.CreateRunAssemblyTask(context, Id),
                    _taskFactory.CreateContextTask(context, Id),
                    _taskFactory.CreateContextSpecificationTask(context, contextSpecification, Id)
                };
            }

            if (this is BehaviorSpecificationElement)
            {
                var behaviorSpecification = this as BehaviorSpecificationElement;
                var behavior = behaviorSpecification.Behavior;
                var context = behavior.Context;

                return new List<UnitTestTask>
                {
                    _taskFactory.CreateRunAssemblyTask(context, Id),
                    _taskFactory.CreateContextTask(context, Id),
                    _taskFactory.CreateBehaviorSpecificationTask(context, behaviorSpecification, Id)
                };
            }

            if (this is ContextElement || this is BehaviorElement)
                return EmptyArray<UnitTestTask>.Instance;

            throw new ArgumentException($"Element is not a Machine.Specifications element: '{this}'");
        }

        public IUnitTestRunStrategy GetRunStrategy(IHostProvider hostProvider)
        {
            return RunStrategy;
        }

        protected virtual string GetTitlePrefix()
        {
            return string.Empty;
        }

        protected ITypeElement GetDeclaredType()
        {
            return _serviceProvider.CachingService.GetTypeElement(Id.Project, TargetFrameworkId.Default, _declaringTypeName, true, true);
        }

        public IClrTypeName GetTypeClrName()
        {
            return _declaringTypeName;
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
                    thisFullName = _declaringTypeName.FullName;
                    otherFullName = element._declaringTypeName.FullName;
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
            result = 29 * result + _declaringTypeName.FullName.GetHashCode();

            return result;
        }

        public virtual IEnumerable<UnitTestElementLocation> GetLocations(IDeclaredElement element)
        {
            var locations = new List<UnitTestElementLocation>();

            element.GetDeclarations()
                .ToList()
                .ForEach(declaration =>
                {
                    IFile file = declaration.GetContainingFile();

                    if (file != null)
                    {
                        locations.Add(new UnitTestElementLocation(
                            file.GetSourceFile().ToProjectFile(),
                            declaration.GetNameDocumentRange().TextRange,
                            declaration.GetDocumentRange().TextRange));
                    }
                });

            return locations;
        }
    }
}