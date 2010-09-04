using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule
{
    public class LoadWhenFilteredController : ExpressApp.SystemModule.LoadWhenFilteredController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            if (IsReady())
            {
                Frame.GetController<FilterControlListViewController>().CustomAssignFilterControlSourceControl += OnCustomAssignFilterControlSourceControl;
            }
        }

        void OnCustomAssignFilterControlSourceControl(object sender, EventArgs eventArgs)
        {
            ((IFilteredComponentBase)View.Control).RowFilterChanged += OnRowFilterChanged;
        }

        void OnRowFilterChanged(object sender, EventArgs eventArgs)
        {
            var criteriaOperator = Frame.GetController<FilterControlListViewController>().XpandFilterControl.FilterCriteria;
            if (ReferenceEquals(criteriaOperator, null))
            {
                View.CollectionSource.Criteria[LoadWhenFiltered] = GetDoNotLoadWhenFilterExistsCriteria();
            }
            else
            {
                ClearDoNotLoadWhenFilterExistsCriteria();
            }
        }


        protected override string GetActiveFilter()
        {
            return ((IModelListViewWin)View.Model).ActiveFilterString;
        }
    }
}