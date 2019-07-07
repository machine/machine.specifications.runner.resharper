using JetBrains.ReSharper.Psi;
using Machine.Specifications.Runner.ReSharper.Reflection;

namespace Machine.Specifications.Runner.ReSharper
{
    public static class PsiExtensions
    {
        public static ITypeInfo AsTypeInfo(this ITypeElement type, IDeclaredType declaredType = null)
        {
            return new PsiTypeInfoAdapter(type, declaredType);
        }
        
        public static IAttributeInfo AsAttributeInfo(this IAttributeInstance attribute)
        {
            return new PsiAttributeInfoAdapter(attribute);
        }

        public static IFieldInfo AsFieldInfo(this IField field)
        {
            return new PsiFieldInfoAdapter(field);
        }

        public static bool IsContext(this IDeclaredElement element)
        {
            return element is IClass type && type.AsTypeInfo().IsContext();
        }

        public static bool IsSpecification(this IDeclaredElement element)
        {
            return element is IField field && field.AsFieldInfo().IsSpecification();
        }

        public static bool IsBehavior(this IDeclaredElement element)
        {
            return element is IField field && field.AsFieldInfo().IsBehavior();
        }
    }
}
