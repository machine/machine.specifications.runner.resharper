using System.Collections.Generic;
using System.Linq;
using JetBrains;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework.DotNetCore.Common;
using JetBrains.Util.Extension;

namespace Machine.Specifications.ReSharperProvider
{
    public class MspecTestElementMapper : DotNetCoreTestElementMapper
    {
        private readonly IProject _project;
        private readonly TargetFrameworkId _targetFrameworkId;
        private readonly UnitTestElementFactory _factory;

        public MspecTestElementMapper(
            IProject project,
            TargetFrameworkId targetFrameworkId, 
            UnitTestElementFactory factory) 
            : base(project, targetFrameworkId)
        {
            _project = project;
            _targetFrameworkId = targetFrameworkId;
            _factory = factory;
        }

        protected override IUnitTestElement Map(Test test)
        {
            using (ReadLockCookie.Create())
            {
                var assemblyPath = _project?.GetOutputFilePath(_targetFrameworkId);
                var typeName = GetType(test);
                var fieldName = GetField(test);
                var subject = GetTraits(test, "Subject").FirstOrDefault();
                var tags = GetTraits(test, "Tag");
                var behaviorField = GetTraits(test, "BehaviorField").FirstOrDefault();
                var behaviorType = GetTraits(test, "BehaviorType").FirstOrDefault();

                var type = new ClrTypeName(typeName);

                var context = _factory.GetOrCreateContext(type, assemblyPath, subject, tags.ToArray(), false);

                if (!string.IsNullOrEmpty(behaviorField) && !string.IsNullOrEmpty(behaviorType))
                {
                    var behavior = _factory.GetOrCreateBehavior(context, type, behaviorField, false);

                    return _factory.GetOrCreateBehaviorSpecification(behavior, new ClrTypeName(behaviorType), fieldName, false);
                }

                return _factory.GetOrCreateContextSpecification(context, type, fieldName, false);
            }
        }

        private string GetType(Test test)
        {
            return test.FullyQualifiedName.SubstringBeforeLast("::").Trim();
        }

        private string GetField(Test test)
        {
            return test.FullyQualifiedName.SubstringAfterLast("::").Trim();
        }

        private IEnumerable<string> GetTraits(Test test, string category)
        {
            return test.Traits
                .GetValueSafe(category, string.Empty)
                .Split("],[")
                .Select(x => x.Split(',').LastOrDefault())
                .Select(x => x?.Trim('[', ']'))
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Trim());
        }
    }
}
