using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using Machine.Specifications.ReSharperProvider.Elements;

namespace Machine.Specifications.ReSharperProvider.Factories
{
    [SolutionComponent]
    public class ElementCache
    {
        readonly IDictionary<ITypeElement, ContextElement> _contexts;

        public ElementCache()
        {
            _contexts = new Dictionary<ITypeElement, ContextElement>();
        }

        public void AddContext(ITypeElement contextType, ContextElement context)
        {
            if (!_contexts.ContainsKey(contextType))
                _contexts.Add(contextType, context);
            else
                _contexts[contextType] = context;
        }

        public ContextElement TryGetContext(ITypeElement contextType)
        {
            return _contexts.TryGetValue(contextType, out ContextElement context) ? context : null;
        }
    }
}