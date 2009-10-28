using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentReferenceMemberInfo : IPersistentMemberInfo
    {
        Type ReferenceType { get; set; }
        string AssemblyQualifiedName { get; set; }
    }
}