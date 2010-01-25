using System;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class ProvidedAssociationAttribute : Attribute {
        readonly string _attributesFactoryProperty;
        readonly string _providedPropertyName;
        readonly RelationType _relationType;


        public ProvidedAssociationAttribute() : this(null) {
        }

        public ProvidedAssociationAttribute(string providedPropertyName)
            : this(providedPropertyName, RelationType.Undefined, null) {
        }

        public ProvidedAssociationAttribute(RelationType relationType) : this(null, relationType, null) {
        }


        public ProvidedAssociationAttribute(string providedPropertyName, RelationType relationType,
                                            string attributesFactory) {
            _relationType = relationType;
            _attributesFactoryProperty = attributesFactory;
            _providedPropertyName = providedPropertyName;
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