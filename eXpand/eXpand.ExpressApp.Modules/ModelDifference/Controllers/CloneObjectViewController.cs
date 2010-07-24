using System;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public class CloneObjectViewController : DevExpress.ExpressApp.CloneObject.CloneObjectViewController
    {
        protected override void CloneObject(SingleChoiceActionExecuteEventArgs args)
        {
            base.CloneObject(args);
            var modelDifferenceObject = args.ShowViewParameters.CreatedView.CurrentObject as ModelDifferenceObject;
            if (modelDifferenceObject != null) {
                modelDifferenceObject.DateCreated = DateTime.Now;
                modelDifferenceObject.Disabled = true;
                modelDifferenceObject.Name = null;
                modelDifferenceObject.ModelId = Guid.NewGuid().ToString();
                modelDifferenceObject.PersistentApplication = (PersistentApplication) modelDifferenceObject.Session.GetObject(((ModelDifferenceObject)View.CurrentObject).PersistentApplication);
            }
        }
    }
}