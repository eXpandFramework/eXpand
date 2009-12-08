using System;
<<<<<<< HEAD
=======
using System.Collections.Generic;
>>>>>>> CodeDomApproachForWorldCreator

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IInterfaceInfo {
        string Name { get; set; }
        string Assembly { get; set; }
        Type Type { get; }
<<<<<<< HEAD
=======
        IList<IPersistentClassInfo> PersistentClassInfos { get; }
>>>>>>> CodeDomApproachForWorldCreator
    }
}