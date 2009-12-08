using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
<<<<<<< HEAD
    public interface IPersistentMemberInfo : IPersistentTypeInfo
=======
    public interface IPersistentMemberInfo : IPersistentTemplatedTypeInfo
>>>>>>> CodeDomApproachForWorldCreator
    {    
        IPersistentClassInfo Owner { get; set; }
    }
}