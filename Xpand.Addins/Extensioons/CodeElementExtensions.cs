using System;
using System.Linq;
using DevExpress.CodeRush.StructuralParser;
using Attribute = DevExpress.CodeRush.StructuralParser.Attribute;

namespace XpandAddIns.Extensioons {
    public static class CodeElementExtensions {
        public static Attribute FindAttribute(this CodeElement codeElement, Type type) {
            return codeElement.AttributeCount != 0 ? codeElement.Attributes.Cast<Attribute>().FirstOrDefault(attribute => attribute.Is(type)) : null;
        }
    }
}
