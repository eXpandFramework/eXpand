using System.Collections.Generic;
using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
<<<<<<< HEAD
    public interface IPersistentClassInfo:IPersistentTypeInfo {
        Type MergedObjectType { get; set; }
        Type BaseType { get; set; }
        string BaseTypeAssemblyQualifiedName { get; set; }
        IList<IPersistentMemberInfo> OwnMembers { get; }
        IList<IInterfaceInfo> Interfaces { get; }
        Type GetDefaultBaseClass();
        string AssemblyName { get; }
=======
    public interface IPersistentClassInfo:IPersistentTemplatedTypeInfo {
        string BaseTypeFullName { get; }
        Type MergedObjectType { get; set; }
        IList<IPersistentMemberInfo> OwnMembers { get; }
        IList<IInterfaceInfo> Interfaces { get; }
        Type GetDefaultBaseClass();
        IPersistentAssemblyInfo PersistentAssemblyInfo { get; set; }
>>>>>>> CodeDomApproachForWorldCreator
    }
}