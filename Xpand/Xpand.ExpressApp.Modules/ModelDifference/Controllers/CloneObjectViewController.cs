using System;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.Controllers{
    public class CloneObjectViewController : DevExpress.ExpressApp.CloneObject.CloneObjectViewController
    {
        protected override void OnCustomShowClonedObject(DevExpress.ExpressApp.CloneObject.CustomShowClonedObjectEventArgs args)
        {
            base.OnCustomShowClonedObject(args);
            var modelDifferenceObject = args.ClonedObject as ModelDifferenceObject;
            if (modelDifferenceObject != null)
            {
                modelDifferenceObject.DateCreated = DateTime.Now;
                modelDifferenceObject.Disabled = true;
                modelDifferenceObject.Name = null;
                modelDifferenceObject.PersistentApplication = (PersistentApplication)modelDifferenceObject.Session.GetObject(((ModelDifferenceObject)View.CurrentObject).PersistentApplication);

            }
        }
    }
}