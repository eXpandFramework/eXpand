namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentMemberInfo : IPersistentTemplatedTypeInfo
    {    
        IPersistentClassInfo Owner { get; set; }
    }
}