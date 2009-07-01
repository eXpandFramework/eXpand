using System;
using AopAlliance.Aop;

namespace eXpand.ExpressApp.AOP.ObjectSpaceProvider
{
    public class ObjectSpaceProviderDataStoreEventsMixin : IAdvice,IObjectSpaceProviderDataStoreEvents
    {
        public event EventHandler<ObjectSpaceProviderDataStoreArgs> WorkingDataLayerCreated;
        public event EventHandler<ObjectSpaceProviderDataStoreArgs> WorkingDataLayerCreating;
        public void InvokeCreated(ObjectSpaceProviderDataStoreArgs objectSpaceDataStoreArgs)
        {
            EventHandler<ObjectSpaceProviderDataStoreArgs> selectedDataHandler = WorkingDataLayerCreated;
            if (selectedDataHandler != null) selectedDataHandler(this, objectSpaceDataStoreArgs);
        }

        public void InvokeCreating(ObjectSpaceProviderDataStoreArgs objectSpaceDataStoreArgs)
        {
            EventHandler<ObjectSpaceProviderDataStoreArgs> selectedDataHandler = WorkingDataLayerCreating;
            if (selectedDataHandler != null) selectedDataHandler(this, objectSpaceDataStoreArgs);
        }
    }

    public class ObjectSpaceProviderObjectSpaceEventsMixin : IObjectSpaceProviderObjectSpaceEvents, IAdvice
    {


        public void InvokeCreated(ObjectSpaceProviderObjectSpaceArgs objectSpaceArgs)
        {
            EventHandler<ObjectSpaceProviderObjectSpaceArgs> selectedDataHandler = ObjectSpaceCreated;
            if (selectedDataHandler != null) selectedDataHandler(this,objectSpaceArgs);
        }
        public void InvokeCreating(ObjectSpaceProviderObjectSpaceArgs objectSpaceArgs)
        {
            EventHandler<ObjectSpaceProviderObjectSpaceArgs> selectedDataHandler = ObjectSpaceCreating;
            if (selectedDataHandler != null) selectedDataHandler(this,objectSpaceArgs);
        }

        public event EventHandler<ObjectSpaceProviderObjectSpaceArgs> ObjectSpaceCreated;
        public event EventHandler<ObjectSpaceProviderObjectSpaceArgs> ObjectSpaceCreating;
    }
}