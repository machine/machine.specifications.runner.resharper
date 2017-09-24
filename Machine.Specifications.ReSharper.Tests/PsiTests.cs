using System;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    [MspecReferences]
    public abstract class PsiTests : SingleProjectTest
    {
        private readonly PsiElementCollector _collector = new PsiElementCollector();

        public override ICollector Collector => _collector;

        protected override void WithFile(IProject project, string filename, Action<MspecContext> action)
        {
            var file = GetFile(project, filename);

            Assert.That(file, Is.Not.Null, $"Data file not found: {filename}");

            file.ProcessDescendants(_collector);

            var context = new MspecContext(_collector);

            action(context);
        }

        private IFile GetFile(IProject project, string filename)
        {
            return project.GetAllProjectFiles(x => x.Name == filename)
                .FirstOrDefault()?
                .ToSourceFile()?
                .GetTheOnlyPsiFile<CSharpLanguage>();
        }
    }
}
