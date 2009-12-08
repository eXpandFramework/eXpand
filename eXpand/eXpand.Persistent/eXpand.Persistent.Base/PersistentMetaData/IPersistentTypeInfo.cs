using System.Collections.Generic;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentTypeInfo {
        string Name { get; set; }
<<<<<<< HEAD

        IList<IPersistentAttributeInfo> TypeAttributes { get; }
        Session Session { get; }
=======
        IList<IPersistentAttributeInfo> TypeAttributes { get; }
        Session Session { get; }
        
        
>>>>>>> CodeDomApproachForWorldCreator
    }
}