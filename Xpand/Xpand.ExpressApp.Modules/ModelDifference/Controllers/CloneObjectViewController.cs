using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.CloneObject;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public class CloneObjectViewController : ViewController {
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<DevExpress.ExpressApp.CloneObject.CloneObjectViewController>(controller => controller.CustomShowClonedObject += OnCustomShowClonedObject);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<DevExpress.ExpressApp.CloneObject.CloneObjectViewController>(controller => controller.CustomShowClonedObject -= OnCustomShowClonedObject);
        }

        void OnCustomShowClonedObject(object sender, CustomShowClonedObjectEventArgs args) {
            var modelDifferenceObject = args.ClonedObject as ModelDifferenceObject;
            if (modelDifferenceObject != null) {
                modelDifferenceObject.DateCreated = DateTime.Now;
                modelDifferenceObject.Disabled = true;
                modelDifferenceObject.Name = modelDifferenceObject.Name + " "+DateTime.Now;
                modelDifferenceObject.PersistentApplication = (PersistentApplication)modelDifferenceObject.Session.GetObject(((ModelDifferenceObject)View.CurrentObject).PersistentApplication);

            }
        }

    }
}