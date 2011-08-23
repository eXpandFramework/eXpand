using System;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class ProvidedAssociationAttribute : Attribute {
        readonly string _attributesFactoryProperty;
        readonly string _providedPropertyName;
        readonly string _associationName;
        readonly RelationType _relationType;


        public ProvidedAssociationAttribute(string associationName)
            : this(associationName, null) {
        }

        public ProvidedAssociationAttribute(string associationName, string providedPropertyName)
            : this(associationName, providedPropertyName, RelationType.Undefined, null) {
        }

        public ProvidedAssociationAttribute(string associationName, RelationType relationType)
            : this(associationName, null, relationType, null) {
        }


        public ProvidedAssociationAttribute(string associationName, string providedPropertyName, RelationType relationType,
                                            string attributesFactory) {
            _associationName = associationName;
            _relationType = relationType;
            _attributesFactoryProperty = attributesFactory;
            _providedPropertyName = providedPropertyName;
        }

        public string AssociationName {
            get { return _associationName; }
        }

        public string AttributesFactoryProperty {
            get { return _attributesFactoryProperty; }
        }

        public string ProvidedPropertyName {
            get { return _providedPropertyName; }
        }


        public RelationType RelationType {
            get { return _relationType; }
        }
    }
}