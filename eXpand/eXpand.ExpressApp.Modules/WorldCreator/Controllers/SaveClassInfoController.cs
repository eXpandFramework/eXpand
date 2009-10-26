using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    public partial class SaveClassInfoController : ViewController
    {
        public SaveClassInfoController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IPersistentClassInfo);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.Committed+=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs args) {
//            WorldCreatorModule.SyncModel((IPersistentClassInfo)View.CurrentObject, Application.Model);
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
        }
    }
}
