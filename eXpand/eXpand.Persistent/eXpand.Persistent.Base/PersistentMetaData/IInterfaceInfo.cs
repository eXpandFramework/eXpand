using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IInterfaceInfo {
        string Name { get; set; }
        string Assembly { get; set; }
        Type Type { get; }
    }
}