﻿namespace Machine.Specifications.ReSharperProvider
{
    using System.Diagnostics;
    using System.Drawing;

    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.TaskRunnerFramework;
    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Presentation;
    using Machine.Specifications.ReSharperProvider.Properties;
    using Machine.Specifications.ReSharperRunner;

    [UnitTestProvider]
    public class MSpecUnitTestProvider : IUnitTestProvider
    {
        const string ProviderId = "Machine.Specifications";

        readonly UnitTestElementComparer _unitTestElementComparer = new UnitTestElementComparer(typeof(ContextElement),
                                                                                                typeof(BehaviorElement),
                                                                                                typeof(BehaviorSpecificationElement),
                                                                                                typeof(ContextSpecificationElement));

        public MSpecUnitTestProvider()
        {
            Debug.Listeners.Add(new DefaultTraceListener());
        }

        public string ID
        {
            get { return ProviderId; }
        }

        public string Name
        {
            get { return this.ID; }
        }

        public Image Icon
        {
            get { return Resources.Logo; }
        }

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            return this._unitTestElementComparer.Compare(x, y);
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Test:
                    return element is ContextSpecificationElement || element is BehaviorSpecificationElement;
                case UnitTestElementKind.TestContainer:
                    return element is ContextElement || element is BehaviorElement;
                case UnitTestElementKind.TestStuff:
                    return element is ContextSpecificationElement || element is BehaviorSpecificationElement ||
                           element is ContextElement || element is BehaviorElement;
                case UnitTestElementKind.Unknown:
                    return !(element is ContextSpecificationElement || element is BehaviorSpecificationElement ||
                             element is ContextElement || element is BehaviorElement);
            }

            return false;
        }

        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Test:
                    return declaredElement.IsSpecification();
                case UnitTestElementKind.TestContainer:
                    return declaredElement.IsContext() || declaredElement.IsBehavior();
                case UnitTestElementKind.TestStuff:
                    return declaredElement.IsSpecification() || declaredElement.IsContext() || declaredElement.IsBehavior();
                case UnitTestElementKind.Unknown:
                    return !(declaredElement.IsSpecification() || declaredElement.IsContext() || declaredElement.IsBehavior());
            }

            return false;
        }

        public bool IsSupported(IHostProvider hostProvider)
        {
            return true;
        }
    }
}