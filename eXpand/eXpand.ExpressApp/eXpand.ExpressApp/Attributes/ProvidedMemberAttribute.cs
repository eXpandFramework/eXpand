using System;

namespace eXpand.ExpressApp.Attributes{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProvidedMemberAttribute : Attribute{
        public ProvidedMemberAttribute(string providedPropertyName, Type provisionedClassType, params Attribute[] attributes){
            ProvidedPropertyName = providedPropertyName;
            ProvisionedClassType = provisionedClassType;
            ProvisionedAttributes = attributes;
        }

        public ProvidedMemberAttribute(string providedPropertyName, Type provisionedClassType){
            ProvidedPropertyName = providedPropertyName;
            ProvisionedClassType = provisionedClassType;
        }

        public string ProvidedPropertyName { get; set; }
        public Type ProvisionedClassType { get; set; }
        public Attribute[] ProvisionedAttributes { get; set; }
        
    }
}