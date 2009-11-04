namespace eXpand.ExpressApp {
    public interface IObjectSpaceProvider {
        IXpoDataStoreProxy DataStoreProvider { get; set; }
    }
}