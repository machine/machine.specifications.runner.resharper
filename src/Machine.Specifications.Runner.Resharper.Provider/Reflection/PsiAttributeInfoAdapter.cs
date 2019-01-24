﻿using System.Collections.Generic;
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

            var typeItems = parameters
                .Where(x => x.IsType)
                .Select(x => x.TypeValue)
                .OfType<IDeclaredType>()
                .Where(x => x.IsValid())
                .Select(x => x.GetClrName().ShortName);

            var arrayItems = parameters
                .Where(x => x.IsArray)
                .SelectMany(x => x.ArrayValue);

            var stringItems = constantItems
                .Concat(arrayItems)
                .Select(x => x.ConstantValue.Value)
                .Where(x => x != null)
                .Select(x => x.ToString());

            return typeItems.Concat(stringItems);
        }
    }
}
