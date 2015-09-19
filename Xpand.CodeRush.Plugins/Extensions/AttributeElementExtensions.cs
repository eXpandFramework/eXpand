using System;
using DevExpress.CodeRush.StructuralParser;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class AttributeElementExtensions {
        public static bool Is(this IAttributeElement attributeElement, Type type) {
            string fullName = attributeElement.GetDeclaration(false).FullName;
            return fullName == type.FullName;
        }

    }
}
