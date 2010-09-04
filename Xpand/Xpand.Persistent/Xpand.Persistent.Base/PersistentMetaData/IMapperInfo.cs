namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IMapperInfo {
        string NavigationPath { get; set; }
        bool XpoDataBase { get; set; }
    }
}