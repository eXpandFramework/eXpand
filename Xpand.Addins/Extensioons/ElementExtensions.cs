using System;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.StructuralParser;

namespace XpandAddIns.Extensioons
{
    public static class ElementExtensions
    {

        public static bool Is(this IElement element,Type type)
        {
            IElement resolveType = element.ResolveType();
            IElement declaration = resolveType.GetDeclaration();
            ITypeElement findType = CodeRush.Source.FindType(declaration.FullName);
            return findType.Is(type);
        }

        public static IElement ResolveType(this IElement active)
        {
            ISourceTreeResolver resolver = ParserServices.SourceTreeResolver;
            if (active is IMemberElement)
                return resolver.ResolveElementType(active);

            var expression = active as IExpression;
            if (expression != null)
                return resolver.ResolveExpression(expression);

            
            return null;
        }
    }
}
