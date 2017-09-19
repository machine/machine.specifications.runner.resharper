using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace Machine.Specifications.ReSharperProvider.Reflection
{
    public class PsiFieldInfoAdapter : IFieldInfo
    {
        private readonly IField _field;

        public PsiFieldInfoAdapter(IField field)
        {
            _field = field;
        }

        public ITypeInfo FieldType
        {
            get
            {
                if (_field.Type.IsResolved)
                {
                    if (_field.Type is IDeclaredType declaredType)
                        return declaredType.AsTypeInfo();

                    if (_field.Type.GetTypeElement() is IClass classType)
                        return classType.AsTypeInfo();
                }

                return UnknownTypeInfoAdapter.Default;
            }
        }

        public string DeclaringType => _field.GetContainingType()?.GetClrName().FullName;

        public string ShortName => _field.ShortName;

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string typeName)
        {
            return _field.GetAttributeInstances(false)
                .Where(x => x.GetClrName().FullName == typeName)
                .Select(x => x.AsAttributeInfo());
        }
    }
}
