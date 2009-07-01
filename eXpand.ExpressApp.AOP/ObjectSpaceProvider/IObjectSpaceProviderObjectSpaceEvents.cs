using System;

namespace eXpand.ExpressApp.AOP.ObjectSpaceProvider
{
    public interface IObjectSpaceProviderObjectSpaceEvents
    {
        event EventHandler<ObjectSpaceProviderObjectSpaceArgs> ObjectSpaceCreated;
        event EventHandler<ObjectSpaceProviderObjectSpaceArgs> ObjectSpaceCreating;
        void InvokeCreated(ObjectSpaceProviderObjectSpaceArgs objectSpaceArgs);
        void InvokeCreating(ObjectSpaceProviderObjectSpaceArgs objectSpaceArgs);
    }
}