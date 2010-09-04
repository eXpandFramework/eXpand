using System;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IExtendedReferenceMemberInfo : IExtendedMemberInfo
    {
        Type ReferenceType { get; set; }
    }
}