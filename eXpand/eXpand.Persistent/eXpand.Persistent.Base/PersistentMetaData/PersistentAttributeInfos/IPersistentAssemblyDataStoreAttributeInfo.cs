namespace eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentAssemblyDataStoreAttributeInfo:IPersistentAssemblyAttributeInfo {
        IDataStoreLogonObject DataStoreLogon { get; set; }
        IPersistentClassInfo PersistentClassInfo { get; set; }
    }
}