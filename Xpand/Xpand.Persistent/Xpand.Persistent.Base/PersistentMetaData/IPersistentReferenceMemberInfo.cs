namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentReferenceMemberInfo : IPersistentAssociatedMemberInfo{
        string ReferenceTypeFullName { get; set; }
        void SetReferenceTypeFullName(string fullName);
        IPersistentClassInfo ReferenceClassInfo { get; set; }
    }
}