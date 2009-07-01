using System.ComponentModel;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.AOP.ObjectSpaceProvider
{
    public class ObjectSpaceProviderObjectSpaceArgs:HandledEventArgs
    {
        private readonly IObjectSpaceProvider objectSpaceProvider;

        public ObjectSpace ObjectSpace { get; set; }

        public ObjectSpaceProviderObjectSpaceArgs(IObjectSpaceProvider objectSpaceProvider)
        {
            this.objectSpaceProvider = objectSpaceProvider;
        }

        public IObjectSpaceProvider ObjectSpaceProvider
        {
            get { return objectSpaceProvider; }
        }
    }
}