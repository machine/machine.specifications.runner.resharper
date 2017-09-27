﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.Util;
using Machine.Specifications.ReSharperRunner;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class ContextSpecificationElement : FieldElement, IEquatable<ContextSpecificationElement>
    {
        public ContextSpecificationElement(
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored)
            : base(id, parent, typeName, serviceProvider, fieldName, isIgnored || parent.Explicit)
        {
        }

        public ContextElement Context => Parent as ContextElement;

        public override string Kind => "Specification";

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestRun run)
        {
            return new[]
            {
                new UnitTestTask(null, new MspecTestAssemblyTask(Id.ProjectId, Context.AssemblyLocation.FullPath)),
                new UnitTestTask(Context, new MspecTestContextTask(Id.ProjectId, Context.TypeName.FullName)),
                new UnitTestTask(this, new MspecTestSpecificationTask(Id.ProjectId, Context.TypeName.FullName, FieldName))
            };
        }

        public bool Equals(ContextSpecificationElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(FieldName, other.FieldName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContextSpecificationElement);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(Id)
                .And(TypeName?.FullName)
                .And(FieldName);
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, IUnitTestProvider provider, ContextElement contextElement, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, contextElement.Id.Project, consumer.TargetFrameworkId, id);
        }
    }
}
