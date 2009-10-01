using DevExpress.ExpressApp;

namespace eXpand.ExpressApp
{
    public class ObjectSpaceProvider : DevExpress.ExpressApp.ObjectSpaceProvider
    {
        public DataStoreProvider DataStoreProvider { get; set; }


        public ObjectSpaceProvider(DataStoreProvider provider)
            : base(provider)
        {
            DataStoreProvider = provider;
        }
    }
}