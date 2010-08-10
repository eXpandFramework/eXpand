using System;

namespace eXpand.Persistent.Base.PersistentMetaData
{
    public class RegistratorAttribute:Attribute
    {
        readonly Type interfaceType;

        public RegistratorAttribute(Type interfaceType) {
            this.interfaceType = interfaceType;
        }

        public Type InterfaceType {
            get { return interfaceType; }
        }
    }
}
