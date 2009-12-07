using System.Collections.Generic;
using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentClassInfo:IPersistentTemplatedTypeInfo {
        string BaseTypeFullName { get; }
        Type MergedObjectType { get; set; }
        IList<IPersistentMemberInfo> OwnMembers { get; }
        IList<IInterfaceInfo> Interfaces { get; }
        Type GetDefaultBaseClass();
        IPersistentAssemblyInfo PersistentAssemblyInfo { get; set; }
    }
}