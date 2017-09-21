using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class PsiAttributeInfoAdapter : IAttributeInfo
    {
        private readonly IAttributeInstance _attribute;

        public PsiAttributeInfoAdapter(IAttributeInstance attribute)
        {
            _attribute = attribute;
        }

        public IEnumerable<string> GetParameters()
        {
            var parameters = _attribute.PositionParameters()
                .Where(x => !x.IsBadValue)
                .ToArray();

            var constantItems = parameters.Where(x => x.IsConstant);

            var arrayItems = parameters
                .Where(x => x.IsArray)
                .SelectMany(x => x.ArrayValue);

            return arrayItems
                .Concat(constantItems)
                .Select(x => x.ConstantValue.Value)
                .Where(x => x != null)
                .Select(x => x.ToString());
        }
    }
}
