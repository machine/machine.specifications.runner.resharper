using System.Diagnostics;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.UI.BindableLinq.Interfaces;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.ReSharper.UnitTestFramework.Strategy;
    using JetBrains.Util;
    using Machine.Specifications.ReSharperProvider.Factories;
    using Machine.Specifications.ReSharperRunner;

    public abstract class Element : IUnitTestElement
    {
        readonly IClrTypeName _declaringTypeName;
        readonly MSpecUnitTestProvider _provider;
        readonly UnitTestTaskFactory _taskFactory;
        readonly UnitTestingCachingService _cachingService;
        readonly IUnitTestElementManager _elementManager;
        IUnitTestElement _parent;

        protected Element(MSpecUnitTestProvider provider,
                          Element parent,
                          IClrTypeName declaringTypeName,
                          UnitTestingCachingService cachingService,
                          IUnitTestElementManager elementManager,
                          bool isIgnored)
        {
            if (declaringTypeName == null)
            {
                throw new ArgumentNullException("declaringTypeName");
            }

            this._provider = provider;
            this._declaringTypeName = declaringTypeName;
            this._cachingService = cachingService;
            this._elementManager = elementManager;

            if (isIgnored)
            {
                this.ExplicitReason = "Ignored";
            }

            this.Parent = parent;

            this.Children = new BindableCollection<IUnitTestElement>(UT.Locks.ReadLock);
            this._taskFactory = new UnitTestTaskFactory(this._provider.ID);
        }

        public abstract string Kind { get; }
        public abstract IEnumerable<UnitTestElementCategory> Categories { get; }
        public string ExplicitReason { get; private set; }

        public abstract UnitTestElementId Id
        {
            get;
        }

        public IUnitTestProvider Provider
        {
            get { return this._provider; }
        }

        public IUnitTestElement Parent
        {
            get { return this._parent; }
            set
            {
                if (Equals(this._parent, value))
                {
                    return;
                }

                var oldParent = this._parent;
                var newParent = value;

                using (UT.WriteLock())
                {
                    if (this._parent != null)
                    {
                        this._parent.Children.Remove(this);
                    }

                    this._parent = newParent;

                    if (this._parent != null)
                    {
                        this._parent.Children.Add(this);
                    }
                }

                this._elementManager.FireElementChanged(oldParent);
                this._elementManager.FireElementChanged(newParent);
            }
        }
        public ICollection<IUnitTestElement> Children { get; private set; }
        public abstract string ShortName { get; }

        public bool Explicit
        {
            get { return false; }
        }

        public UnitTestElementState State { get; set; }

        public IProject GetProject()
        {
            return this.Id.Project;
        }

        public UnitTestElementNamespace GetNamespace()
        {
            return UnitTestElementNamespaceFactory.Create(this._declaringTypeName.NamespaceNames);
        }

        public virtual UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = this.GetDeclaredElement();
            if (element == null || !element.IsValid())
            {
                return UnitTestElementDisposition.InvalidDisposition;
            }

            IEnumerable<UnitTestElementLocation> locations = this.GetLocations(element);
            return new UnitTestElementDisposition(locations, this);
        }

        public abstract string GetPresentation();

        public string GetPresentation(IUnitTestElement unitTestElement, bool full)
        {
            return this.GetPresentation();
        }

        public abstract IDeclaredElement GetDeclaredElement();

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            ITypeElement declaredType = this.GetDeclaredType();
            if (declaredType == null)
            {
                return EmptyArray<IProjectFile>.Instance;
            }

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
                 this._taskFactory.CreateRunAssemblyTask(context),
                 this._taskFactory.CreateContextTask(context),
                 this._taskFactory.CreateContextSpecificationTask(context, contextSpecification)
               };
            }

            if (this is BehaviorSpecificationElement)
            {
                var behaviorSpecification = this as BehaviorSpecificationElement;
                var behavior = behaviorSpecification.Behavior;
                var context = behavior.Context;

                return new List<UnitTestTask>
               {
                 this._taskFactory.CreateRunAssemblyTask(context),
                 this._taskFactory.CreateContextTask(context),
                 this._taskFactory.CreateBehaviorSpecificationTask(context, behaviorSpecification)
               };
            }

            if (this is ContextElement || this is BehaviorElement)
            {
                return EmptyArray<UnitTestTask>.Instance;
            }

            throw new ArgumentException(String.Format("Element is not a Machine.Specifications element: '{0}'", this));
        }

        private static readonly IUnitTestRunStrategy RunStrategy = new OutOfProcessUnitTestRunStrategy(new RemoteTaskRunnerInfo(RecursiveMSpecTaskRunner.RunnerId, typeof(RecursiveMSpecTaskRunner)));

        public IUnitTestRunStrategy GetRunStrategy(IHostProvider hostProvider)
        {
            return RunStrategy;
        }

        public virtual string GetTitlePrefix()
        {
            return String.Empty;
        }

        protected ITypeElement GetDeclaredType()
        {
            return _cachingService.GetTypeElement(Id.Project, this._declaringTypeName, true, true);
        }

        public IClrTypeName GetTypeClrName()
        {
            return this._declaringTypeName;
        }

        public bool Equals(IUnitTestElement other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.GetType() == this.GetType())
            {
                var element = (Element)other;
                string thisFullName;
                string otherFullName;
                try
                {
                    // This might throw for invalid elements.
                    thisFullName = this._declaringTypeName.FullName;
                    otherFullName = element._declaringTypeName.FullName;
                }
                catch (NullReferenceException)
                {
                    Debug.Fail("Should not happen as the type name should be persisted!");
                    return false;
                }

                return Equals(other.Id, this.Id)
                       && other.ShortName == this.ShortName
                       && thisFullName == otherFullName;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as IUnitTestElement);
        }

        public override int GetHashCode()
        {
            var result = 0;
            result = 29 * result + this.Id.GetHashCode();
            result = 29 * result + this.ShortName.GetHashCode();
            result = 29 * result + this._declaringTypeName.FullName.GetHashCode();
            return result;
        }

        public virtual IEnumerable<UnitTestElementLocation> GetLocations(IDeclaredElement element)
        {
            var locations = new List<UnitTestElementLocation>();
            element.GetDeclarations().ForEach(declaration =>
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