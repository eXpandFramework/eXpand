using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IExtendedReferenceMemberInfo : IExtendedMemberInfo
    {
        Type ReferenceType { get; set; }
    }
}