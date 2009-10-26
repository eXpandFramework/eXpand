using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentAttributeInfo {
        Attribute Create();
        IPersistentTypeInfo Owner { get; set; }
    }
}