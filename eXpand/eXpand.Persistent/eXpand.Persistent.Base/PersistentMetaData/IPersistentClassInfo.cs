using System.Collections.Generic;
using System;
using DevExpress.Xpo.Metadata;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentClassInfo:IPersistentTypeInfo {
        Type BaseType { get; set; }
        IList<IPersistentMemberInfo> OwnMembers { get; }
        Type GetDefaultBaseClass();
        XPClassInfo PersistentTypeClassInfo { get; }
        string AssemblyName { get; }
    }
}