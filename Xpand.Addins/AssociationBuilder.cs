using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.StructuralParser;
using DevExpress.DXCore.Controls.Xpo;
using XpandAddIns.Extensions;
using Attribute = DevExpress.CodeRush.StructuralParser.Attribute;

namespace XpandAddIns {
    public class AssociationBuilder {
        private readonly Property property;

        public AssociationBuilder(Property property) {
            this.property = property;
        }

        public Class FindAssociatedClass(Avaliabiltity avaliabiltity) {
            switch (avaliabiltity) {
                case Avaliabiltity.XpoManyPart:
                    return (Class)property.ResolveType().GetDeclaration();
                case Avaliabiltity.XpoOnePart:
                    return (Class)((TypeReferenceExpression)property.MemberTypeReference.DetailNodes[0]).GetDeclaration();
                case Avaliabiltity.None:
                    return null;
            }
            return null;
        }

        public string GetAssociationName() {
            Attribute attribute = property.FindAttribute(typeof(AssociationAttribute));
            if (attribute.ArgumentCount > 0) {
                var expression = attribute.Arguments[0] as PrimitiveExpression;
                if (expression != null && expression.IsStringLiteral && expression.TestValue is string) {
                    return expression.TestValue.ToString();
                }
            }
            return null;
        }
        public Attribute CreateOtherPartAssociationAttribute() {
            return (Attribute)property.FindAttribute(typeof(AssociationAttribute)).Clone();
        }

        public Property BuildAssociatedProperty(Avaliabiltity avaliabiltity) {
            Class @class = FindAssociatedClass(avaliabiltity);
            CodeRush.File.Activate(((IElement)@class).FirstFile.Name);
            var associatedClass = (Class)@class.Clone();
            if (avaliabiltity == Avaliabiltity.XpoOnePart) {
                var property1 = new Property(associatedClass.FullName, associatedClass.Name);
                property1.Attributes.Add(CreateOtherPartAssociationAttribute());
                associatedClass.AddNode(property1);
                CodeRush.Documents.ActiveTextDocument.Replace(((IElement)@class).FirstRange, CodeRush.Language.GenerateElement(associatedClass), "add property",
                                                              true);

            }
            return null;
        }
    }
}