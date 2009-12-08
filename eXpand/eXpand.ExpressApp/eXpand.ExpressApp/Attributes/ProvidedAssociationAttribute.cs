using System;

namespace eXpand.ExpressApp.Attributes{
    [AttributeUsage(AttributeTargets.Property)]
    public class ProvidedAssociationAttribute : Attribute {
        private readonly RelationType _relationType;
        private readonly string _attributesFactoryProperty;


        public ProvidedAssociationAttribute():this(null)
        {
        }

        public ProvidedAssociationAttribute(string providedPropertyName):this(providedPropertyName, RelationType.Undefined,null)
        {
        }
        public ProvidedAssociationAttribute(RelationType relationType):this(null, relationType, null)
        {
        }


        public ProvidedAssociationAttribute(string providedPropertyName, RelationType relationType, string attributesFactory)
        {
            _relationType = relationType;
            _attributesFactoryProperty = attributesFactory;
            _providedPropertyName = providedPropertyName;
        }

        public string AttributesFactoryProperty
        {
            get { return _attributesFactoryProperty; }
        }

        private readonly string _providedPropertyName;
        public string ProvidedPropertyName
        {
            get { return _providedPropertyName; }
        }

        

        public RelationType RelationType
        {
            get { return _relationType; }
        }
    }
    public enum RelationType
    {
        Undefined,
        ManyToMany
    }
}