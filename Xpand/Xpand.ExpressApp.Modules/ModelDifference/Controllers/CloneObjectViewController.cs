using System;
using DevExpress.ExpressApp.CloneObject;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public class CloneObjectViewController : DevExpress.ExpressApp.CloneObject.CloneObjectViewController {
        protected override void OnActivated() {
            base.OnActivated();
            CustomShowClonedObject += OnCustomShowClonedObject;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            CustomShowClonedObject -= OnCustomShowClonedObject;
        }

        void OnCustomShowClonedObject(object sender, CustomShowClonedObjectEventArgs args) {
            var modelDifferenceObject = args.ClonedObject as ModelDifferenceObject;
            if (modelDifferenceObject != null) {
                modelDifferenceObject.DateCreated = DateTime.Now;
                modelDifferenceObject.Disabled = true;
                modelDifferenceObject.Name = modelDifferenceObject.Name + " Cloned";
                modelDifferenceObject.PersistentApplication = (PersistentApplication)modelDifferenceObject.Session.GetObject(((ModelDifferenceObject)View.CurrentObject).PersistentApplication);

            }
        }

    }
}