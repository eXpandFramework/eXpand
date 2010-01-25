namespace eXpand.ExpressApp
{
    public class ObjectSpaceProvider : DevExpress.ExpressApp.ObjectSpaceProvider, IObjectSpaceProvider {
        public IXpoDataStoreProxy DataStoreProvider { get; set; }


        public ObjectSpaceProvider(IXpoDataStoreProxy provider)
            : base(provider)
        {
            DataStoreProvider = provider;
        }
    }
}