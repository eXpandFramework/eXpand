using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.SystemModule {
    public class ObjectCreatedEventArgs:ObjectManipulatingEventArgs {
        public ObjectCreatedEventArgs(object theObject) : base(theObject) {
        }
    }
    public class TrackObjectConstructionsViewController:ViewController {
        public event EventHandler<ObjectCreatedEventArgs> ObjectCreated;

        protected void InvokeObjectCreated(ObjectCreatedEventArgs e)
        {
            EventHandler<ObjectCreatedEventArgs> handler = ObjectCreated;
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
                    InvokeObjectCreated(new ObjectCreatedEventArgs(currentObject));
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