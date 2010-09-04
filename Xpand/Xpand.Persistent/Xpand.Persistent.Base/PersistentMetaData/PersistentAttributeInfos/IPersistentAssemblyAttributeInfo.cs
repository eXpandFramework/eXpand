namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentAssemblyAttributeInfo : IPersistentAttributeCreator{
        IPersistentAssemblyInfo Owner { get; set; }
    }
}