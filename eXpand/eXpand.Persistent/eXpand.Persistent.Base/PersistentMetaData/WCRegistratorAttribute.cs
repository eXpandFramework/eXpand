using System;

namespace eXpand.Persistent.Base.PersistentMetaData
{
    public class WCRegistratorAttribute:Attribute
    {
        readonly Type interfaceType;

        public WCRegistratorAttribute(Type interfaceType) {
            this.interfaceType = interfaceType;
        }

        public Type InterfaceType {
            get { return interfaceType; }
        }
    }
}
