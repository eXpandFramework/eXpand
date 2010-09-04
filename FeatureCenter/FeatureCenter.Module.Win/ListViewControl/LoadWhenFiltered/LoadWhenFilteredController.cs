using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Win.SystemModule;

namespace FeatureCenter.Module.Win.ListViewControl.LoadWhenFiltered
{
    public class LoadWhenFilteredController:ViewController<ListView>
    {
        public LoadWhenFilteredController() {
            TargetViewId = Module.ListViewControl.LoadWhenFiltered.AttributeRegistrator.LoadWhenFiltered_ListView;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var filterControlListViewController = Frame.GetController<FilterControlListViewController>();
            filterControlListViewController.FilterControlCreated+=FilterControlListViewControllerOnFilterControlCreated;
            
        }

        void FilterControlListViewControllerOnFilterControlCreated(object sender, EventArgs eventArgs) {
            var filterControlListViewController = Frame.GetController<FilterControlListViewController>();
            var filterControl = filterControlListViewController.XpandFilterControl;
            filterControl.FilterCriteria = new BinaryOperator("City", "Paris");
        }

    }
}
