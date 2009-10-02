using System;
using DevExpress.CodeRush.StructuralParser;
using Attribute=DevExpress.CodeRush.StructuralParser.Attribute;

namespace eXpandAddIns.Extensioons
{
    public static class CodeElementExtensions
    {
        public static Attribute FindAttribute(this CodeElement codeElement, Type type)
        {
            if (codeElement.AttributeCount != 0)
                foreach (Attribute attribute in codeElement.Attributes)
                    if (attribute.Is(type))
                        return attribute;
            return null;
        }

    }
}
