using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider.Explorers.ElementHandlers;
using Machine.Specifications.ReSharperProvider.Factories;

namespace Machine.Specifications.ReSharperProvider.Explorers
{
    public class FileExplorer : IRecursiveElementProcessor
    {
        private readonly IUnitTestElementsObserver _consumer;
        private readonly IEnumerable<IElementHandler> _elementHandlers;
        private readonly IFile _file;
        private readonly Func<bool> _interrupted;
        private readonly FileSystemPath _assemblyPath;

        public FileExplorer(MspecTestProvider provider,
                            ElementFactories factories,
                            IFile file,
                            IUnitTestElementsObserver consumer,
                            Func<bool> interrupted)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            _file = file ?? throw new ArgumentNullException(nameof(file));

            _consumer = consumer;
            _interrupted = interrupted;

            var project = file.GetSourceFile().ToProjectFile().GetProject();

            _assemblyPath = project.GetOutputFilePath(consumer.TargetFrameworkId);

            _elementHandlers = new List<IElementHandler>
            {
                new ContextElementHandler(factories, consumer),
                new ContextSpecificationElementHandler(factories, consumer),
                new BehaviorElementHandler(factories, consumer)
            };
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            if (element is ITypeMemberDeclaration)
                return element is ITypeDeclaration;

            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            IElementHandler handler = _elementHandlers.FirstOrDefault(x => x.Accepts(element));

            if (handler == null)
                return;

            foreach (UnitTestElementDisposition elementDisposition in handler.AcceptElement(_assemblyPath, _file, element))
            {
                if (elementDisposition?.UnitTestElement != null)
                    _consumer.OnUnitTestElementDisposition(elementDisposition);
            }
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        public bool ProcessingIsFinished
        {
            get
            {
                if (_interrupted())
                    throw new ProcessCancelledException();

                return false;
            }
        }
    }
}