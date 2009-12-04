namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentReferenceMemberInfo : IPersistentMemberInfo
    {
        string ReferenceTypeFullName { get; set; }
    }
}