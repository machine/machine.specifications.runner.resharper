namespace Machine.Specifications.ReSharperProvider.Explorers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Application.Progress;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Explorers.ElementHandlers;
    using Machine.Specifications.ReSharperProvider.Factories;

    public class FileExplorer : IRecursiveElementProcessor
    {
        readonly IUnitTestElementsObserver _consumer;
        readonly IEnumerable<IElementHandler> _elementHandlers;
        readonly IFile _file;
        readonly Func<bool> _interrupted;
        readonly string _assemblyPath;

        public FileExplorer(MSpecUnitTestProvider provider,
                            ElementFactories factories,
                            IFile file,
                            IUnitTestElementsObserver consumer,
                            Func<bool> interrupted)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._consumer = consumer;
            this._file = file;
            this._interrupted = interrupted;

            var project = file.GetSourceFile().ToProjectFile().GetProject();

            this._assemblyPath = project.GetOutputFilePath().FullPath;

            this._elementHandlers = new List<IElementHandler>
                         {
                           new ContextElementHandler(factories),
                           new ContextSpecificationElementHandler(factories),
                           new BehaviorElementHandler(factories)
                         };
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            if (element is ITypeMemberDeclaration)
            {
                return element is ITypeDeclaration;
            }

            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            IElementHandler handler = this._elementHandlers.FirstOrDefault(x => x.Accepts(element));
            if (handler == null)
            {
                return;
            }

            foreach (UnitTestElementDisposition elementDisposition in handler.AcceptElement(this._assemblyPath, this._file, element))
            {
                if (elementDisposition != null && elementDisposition.UnitTestElement != null)
                {
                    this._consumer.OnUnitTestElementDisposition(elementDisposition);
                }
            }
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        public bool ProcessingIsFinished
        {
            get
            {
                if (this._interrupted())
                {
                    throw new ProcessCancelledException();
                }

                return false;
            }
        }
    }
}