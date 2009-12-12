using System;
using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.SystemModule {
    public class ObjectCreatedEventArgs:ObjectManipulatingEventArgs {
        public ObjectCreatedEventArgs(object theObject) : base(theObject) {
        }
    }
    public class _TrackObjectConstructionsViewController:ViewController {
        public event EventHandler<ObjectCreatedEventArgs> ObjectConstructed;

        public _TrackObjectConstructionsViewController() {
            TargetObjectType = typeof (IAfterConstructed);
        }

        protected virtual void OnObjectCreated(ObjectCreatedEventArgs e)
        {
            EventHandler<ObjectCreatedEventArgs> handler = ObjectConstructed;
            if (handler != null) handler(this, e);
        }


        protected override void OnActivated()
        {
            base.OnActivated();
            Application.ViewShown += Application_ViewShown;
        }




        private void Application_ViewShown(object sender, ViewShownEventArgs e)
        {
            if (e.TargetFrame != null && e.TargetFrame.View != null) {
                var currentObject = e.TargetFrame.View.CurrentObject;
                if (currentObject != null && e.TargetFrame.View.ObjectSpace.Session.IsNewObject(currentObject)) {
                    OnObjectCreated(new ObjectCreatedEventArgs(currentObject));
                }
            }
        }

        protected override void OnDeactivating()
        {
            Application.ViewShown -= Application_ViewShown;
            base.OnDeactivating();
        }
    }
}