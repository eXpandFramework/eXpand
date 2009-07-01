using System;
using System.ComponentModel;
using DevExpress.Xpo.DB;

namespace eXpand.ExpressApp.AOP.ObjectSpaceProvider
{
    public interface IObjectSpaceProviderDataStoreEvents
    {
        event EventHandler<ObjectSpaceProviderDataStoreArgs> WorkingDataLayerCreated;
        event EventHandler<ObjectSpaceProviderDataStoreArgs> WorkingDataLayerCreating;
        void InvokeCreated(ObjectSpaceProviderDataStoreArgs objectSpaceDataStoreArgs);
        void InvokeCreating(ObjectSpaceProviderDataStoreArgs objectSpaceDataStoreArgs);
    }

    public class ObjectSpaceProviderDataStoreArgs : HandledEventArgs
    {
        public ObjectSpaceProviderDataStoreArgs(IDataStore dataStore)
        {
            DataStore = dataStore;
        }

        public IDataStore DataStore { get; set; }
    }
}