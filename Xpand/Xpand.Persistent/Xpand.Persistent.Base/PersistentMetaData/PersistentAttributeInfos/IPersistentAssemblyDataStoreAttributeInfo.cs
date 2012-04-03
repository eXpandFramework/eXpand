namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentAssemblyDataStoreAttributeInfo : IPersistentAssemblyAttributeInfo {
        IDataStoreLogonObject DataStoreLogon { get; set; }
        IPersistentClassInfo PersistentClassInfo { get; set; }
    }

    public interface IPersistentAssemblyDataStoreAttribute : IPersistentAssemblyAttributeInfo {
        IPersistentClassInfo PersistentClassInfo { get; set; }
        string ConnectionString { get; set; }
        bool IsLegacy { get; set; }
    }
}