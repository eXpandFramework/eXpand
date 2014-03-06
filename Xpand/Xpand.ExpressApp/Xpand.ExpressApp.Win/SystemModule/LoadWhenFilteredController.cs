using System;
using DevExpress.Data.Filtering;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class LoadWhenFilteredController : ExpressApp.SystemModule.LoadWhenFilteredController {
        protected override void OnActivated() {
            base.OnActivated();
            if (IsReady()) {
                Frame.GetController<FilterControlListViewController>().CustomAssignFilterControlSourceControl += OnCustomAssignFilterControlSourceControl;
            }
        }

        void OnCustomAssignFilterControlSourceControl(object sender, EventArgs eventArgs){
            var filteredComponentBase = View.Control as IFilteredComponentBase;
            if (filteredComponentBase != null) filteredComponentBase.RowFilterChanged += OnRowFilterChanged;
        }

        void OnRowFilterChanged(object sender, EventArgs eventArgs) {
            var criteriaOperator = Frame.GetController<FilterControlListViewController>().FilterControl.FilterCriteria;
            if (ReferenceEquals(criteriaOperator, null)) {
                View.CollectionSource.Criteria[LoadWhenFiltered] = GetDoNotLoadWhenFilterExistsCriteria();
            } else {
                ClearDoNotLoadWhenFilterExistsCriteria();
            }
        }
    }
}