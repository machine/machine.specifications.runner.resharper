using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    using JetBrains.ProjectModel;
    using Machine.Specifications.ReSharperProvider.Presentation;
    using System.Collections.Generic;

    [SolutionComponent]
    public class ElementCache
    {
        readonly IDictionary<ITypeElement, ContextElement> _contexts;

        public ElementCache()
        {
            this._contexts = new Dictionary<ITypeElement, ContextElement>();
        }

        public void AddContext(ITypeElement contextType, ContextElement context)
        {
            if (!this._contexts.ContainsKey(contextType))
            {
                this._contexts.Add(contextType, context);
            }
            else
            {
                this._contexts[contextType] = context;
            }
        }

        public ContextElement TryGetContext(ITypeElement contextType)
        {
            ContextElement context;
            return this._contexts.TryGetValue(contextType, out context) ? context : null;
        }

    }
}