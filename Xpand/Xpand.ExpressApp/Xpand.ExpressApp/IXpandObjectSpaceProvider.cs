namespace Xpand.ExpressApp {
    public interface IXpandObjectSpaceProvider {
        IXpoDataStoreProxy DataStoreProvider { get; set; }
    }
}