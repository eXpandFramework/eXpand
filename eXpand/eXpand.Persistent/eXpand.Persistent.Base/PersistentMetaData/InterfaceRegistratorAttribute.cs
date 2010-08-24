using System;

namespace eXpand.Persistent.Base.PersistentMetaData
{
    public class InterfaceRegistratorAttribute:Attribute
    {
        readonly Type interfaceType;

        public InterfaceRegistratorAttribute(Type interfaceType) {
            this.interfaceType = interfaceType;
        }

        public Type InterfaceType {
            get { return interfaceType; }
        }
    }
}
