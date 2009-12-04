using System;
using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.SystemModule {
    public class NewObjectSavedEventArgs : ObjectManipulatingEventArgs {
        public NewObjectSavedEventArgs(object theObject)
            : base(theObject) {
        }
    }

    public class TrackNewObjectSavedController : ViewController<DetailView> {
        bool isNewObject;

        public TrackNewObjectSavedController() {
            
            TargetObjectType = typeof (IPersistentAttributeInfo);
        }

        public event EventHandler<NewObjectSavedEventArgs> NewObjectSaved;

        protected void InvokeNewObjectSaved(NewObjectSavedEventArgs e) {
            EventHandler<NewObjectSavedEventArgs> handler = NewObjectSaved;
            if (handler != null) handler(this, e);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectSaving += ObjectSpaceOnObjectSaving;
            ObjectSpace.ObjectSaved += ObjectSpaceOnObjectSaved;
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            ObjectSpace.ObjectSaving -= ObjectSpaceOnObjectSaving;
            ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaved;
        }
        void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs objectManipulatingEventArgs) {
            if (isNewObject)
                InvokeNewObjectSaved(new NewObjectSavedEventArgs(objectManipulatingEventArgs.Object));
        }

        void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs objectManipulatingEventArgs) {
            if (objectManipulatingEventArgs.Object == View.CurrentObject) {
                isNewObject = View.ObjectSpace.Session.IsNewObject(View.CurrentObject);
            }
        }
    }
}