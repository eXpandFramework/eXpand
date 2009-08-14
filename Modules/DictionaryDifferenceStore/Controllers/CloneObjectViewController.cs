using System;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Controllers
{
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
            if (args.ShowViewParameters.CreatedView.CurrentObject is XpoModelDictionaryDifferenceStore)
            {
                var store =
                    ((XpoModelDictionaryDifferenceStore) args.ShowViewParameters.CreatedView.CurrentObject);
                store.DateCreated = DateTime.Now;
                store.Disable = true;
                store.Name = null;
            }
        }
    }
}