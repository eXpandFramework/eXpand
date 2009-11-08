using System.Collections.Generic;
using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentClassInfo:IPersistentTypeInfo {
        Type MergedObjectType { get; set; }
        Type BaseType { get; set; }
        string BaseTypeAssemblyQualifiedName { get; set; }
        IList<IPersistentMemberInfo> OwnMembers { get; }
        IList<IInterfaceInfo> Interfaces { get; }
        Type GetDefaultBaseClass();
        string AssemblyName { get; }
    }
}