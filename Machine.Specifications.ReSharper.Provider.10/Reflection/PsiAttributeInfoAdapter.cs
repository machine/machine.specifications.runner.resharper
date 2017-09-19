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

            var typeValues = parameters
                .Where(x => x.IsType)
                .Select(x => x.TypeValue)
                .OfType<IDeclaredType>()
                .Where(x => x.IsValid())
                .Select(x => x.GetClrName().ShortName);

            var arrayValues = parameters
                .Where(x => x.IsArray)
                .SelectMany(x => x.ArrayValue)
                .Select(x => x.ConstantValue.Value?.ToString());

            var constantValues = parameters
                .Where(x => x.IsConstant)
                .Select(x => x.ConstantValue.Value?.ToString());

            return typeValues.Concat(arrayValues).Concat(constantValues);
        }
    }
}
