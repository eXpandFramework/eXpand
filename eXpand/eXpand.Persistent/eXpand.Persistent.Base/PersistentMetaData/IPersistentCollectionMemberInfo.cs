using System;
namespace eXpand.Persistent.Base.PersistentMetaData
{
    public interface IPersistentCollectionMemberInfo:IPersistentMemberInfo
    {
        string CollectionTypeFullName { get; }
    }
}
