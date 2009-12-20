namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentReferenceMemberInfo : IPersistentAssociatedMemberInfo{
        string ReferenceTypeFullName { get; set; }
    }
}