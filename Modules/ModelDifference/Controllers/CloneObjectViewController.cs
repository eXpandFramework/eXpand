using System;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public partial class CloneObjectViewController : DevExpress.ExpressApp.CloneObject.CloneObjectViewController
    {
        public CloneObjectViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void CloneObject(SingleChoiceActionExecuteEventArgs args)
        {
            base.CloneObject(args);
            if (args.ShowViewParameters.CreatedView.CurrentObject is ModelDifferenceObject)
            {
                var store =
                    ((ModelDifferenceObject) args.ShowViewParameters.CreatedView.CurrentObject);
                store.DateCreated = DateTime.Now;
                store.Disabled = true;
                store.Name = null;
            }
        }
    }
}