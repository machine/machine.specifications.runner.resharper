using JetBrains.ReSharper.TaskRunnerFramework;

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
        Element _parent;
        readonly UnitTestingCachingService _cachingService;
        private UnitTestElementState _state;

        protected Element(MSpecUnitTestProvider provider,
                          Element parent,
                          IClrTypeName declaringTypeName,
                          UnitTestingCachingService cachingService,
                          bool isIgnored)
        {
            if (declaringTypeName == null)
            {
                throw new ArgumentNullException("declaringTypeName");
            }

            this._provider = provider;
            this._declaringTypeName = declaringTypeName;
            this._cachingService = cachingService;

            if (isIgnored)
            {
                this.ExplicitReason = "Ignored";
            }

            this.TypeName = declaringTypeName;
            this.Parent = parent;

            this.Children = new List<IUnitTestElement>();
            this._taskFactory = new UnitTestTaskFactory(this._provider.ID);
        }

        public IClrTypeName TypeName { get; private set; }

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
                if (this._parent == value)
                {
                    return;
                }

                if (this._parent != null)
                {
                    this._parent.RemoveChild(this);
                }

                this._parent = (Element)value;
                if (this._parent != null)
                {
                    this._parent.AddChild(this);
                }
            }
        }
        public ICollection<IUnitTestElement> Children { get; private set; }
        public abstract string ShortName { get; }

        public bool Explicit
        {
            get { return false; }
        }

        public UnitTestElementState State
        {
            get
            {
                if (this.Parent == null)
                {
                    return UnitTestElementState.Invalid;
                }

                return _state;
            }
            set { _state = value; }
        }

        public IProject GetProject()
        {
            return this.Id.Project;
        }

        public UnitTestElementNamespace GetNamespace()
        {
            return UnitTestElementNamespaceFactory.Create(this._declaringTypeName.NamespaceNames);
        }

        public UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = this.GetDeclaredElement();
            if (element == null || !element.IsValid())
            {
                return UnitTestElementDisposition.InvalidDisposition;
            }

            var locations = new List<UnitTestElementLocation>();
            element.GetDeclarations().ForEach(declaration =>
            {
                IFile file = declaration.GetContainingFile();
                if (file != null)
                {
                    locations.Add(new UnitTestElementLocation(file.GetSourceFile().ToProjectFile(),
                                                              declaration.GetNameDocumentRange().TextRange,
                                                              declaration.GetDocumentRange().TextRange));
                }
            });

            return new UnitTestElementDisposition(locations, this);
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
                    return false;
                }

                return Equals(other.Id, this.Id)
                       && other.ShortName == this.ShortName
                       && thisFullName == otherFullName;
            }
            return false;
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
            return _cachingService.GetTypeElement(Id.Project, TypeName, true, true);
        }

        public IClrTypeName GetTypeClrName()
        {
            return this._declaringTypeName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Element)) return false;
            return Equals((Element)obj);
        }

        public override int GetHashCode()
        {
            var result = 0;
            result = 29 * result + this.ShortName.GetHashCode();
            result = 29 * result + this.Id.GetHashCode();
            return result;
        }

        void AddChild(Element behaviorElement)
        {
            this.Children.Add(behaviorElement);
        }

        void RemoveChild(Element behaviorElement)
        {
            this.Children.Remove(behaviorElement);
        }
    }
}