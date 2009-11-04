using System.Collections.Generic;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentTypeInfo {
        string Name { get; set; }

        IList<IPersistentAttributeInfo> TypeAttributes { get; }
        Session Session { get; }
    }
}