using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using Machine.Specifications.ReSharperProvider;
using Machine.Specifications.ReSharperProvider.Reflection;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    [MspecReferences]
    public abstract class PsiTests : BaseTestWithSingleProject
    {
        private readonly PsiElementCollector _collector = new PsiElementCollector();

        protected override string RelativeTestDataPath => "Psi";

        protected IList<IClass> Classes => _collector.Classes;

        protected IList<IField> Fields => _collector.Fields;

        protected ITypeInfo Type(string name = null)
        {
            var type = string.IsNullOrEmpty(name)
                ? Classes.FirstOrDefault()
                : Classes.FirstOrDefault(x => x.ShortName == name);

            Assert.That(type, Is.Not.Null, $"Type not found: {name}");

            return type.AsTypeInfo();
        }

        protected IFieldInfo Field(string name = null)
        {
            var field = string.IsNullOrEmpty(name)
                ? Fields.FirstOrDefault()
                : Fields.FirstOrDefault(x => x.ShortName == name);

            Assert.That(field, Is.Not.Null, $"Field not found: {name}");

            return field.AsFieldInfo();
        }

        protected void WithPsiFile(string filename, Action<IFile> action)
        {
            WithSingleProject(filename, (lifetime, solution, project) =>
            {
                Locks.ReentrancyGuard.Execute(nameof(PsiTests), () =>
                {
                    using (ReadLockCookie.Create())
                    {
                        var file = GetFile(project, filename);

                        Assert.That(file, Is.Not.Null, $"Data file not found: {filename}");

                        _collector.Reset();
                        file.ProcessDescendants(_collector);

                        action(file);
                    }
                });
            });
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
