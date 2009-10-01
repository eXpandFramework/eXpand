using System;
using eXpand.ExpressApp.Enums;

namespace eXpand.ExpressApp.Attributes{
    [AttributeUsage(AttributeTargets.Property)]
    public class ProvidedAssociationAttribute : Attribute {
        public ProvidedAssociationAttribute(string providedPropertyName, Type providedAssociationMemberType)
            : this(providedPropertyName, providedAssociationMemberType, ManyToManyInfoType.Undefined) {}

        public ProvidedAssociationAttribute(string providedPropertyName) : 
            this(providedPropertyName, null, ManyToManyInfoType.Undefined) {}

        public ProvidedAssociationAttribute(string providedPropertyName, Type providedAssociationMemberType, ManyToManyInfoType manyToManyInfoType) {
            ProvidedPropertyName = providedPropertyName;
            ManyToManyInfoType = manyToManyInfoType;
            ProvidedAssociationMemberType = providedAssociationMemberType;
        }


        public string ProvidedPropertyName { get; set; }
        public ManyToManyInfoType ManyToManyInfoType { get; set; }
        public Type ProvidedAssociationMemberType { get; set; }
    }
}