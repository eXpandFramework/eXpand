namespace eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentRuleRequiredFieldAttribute:IPersistentAttributeInfo {
        string ID { get; set; }
        string Context { get; set; }
    }
}