using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.MasterDetail.AtAnyLevel {
    public class OrderLinesNewItemRowPosition : ViewController<ListView>
    {
        public OrderLinesNewItemRowPosition()
        {
            TargetViewId = "MasterDetailAtAnyLevelOrderLine_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<NewItemRowListViewController>().CustomCalculateNewItemRowPosition += OnCustomCalculateNewItemRowPosition;
        }

        void OnCustomCalculateNewItemRowPosition(object sender, CustomCalculateNewItemRowPositionEventArgs customCalculateNewItemRowPositionEventArgs)
        {
            customCalculateNewItemRowPositionEventArgs.NewItemRowPosition = NewItemRowPosition.Bottom;

        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.GetController<NewItemRowListViewController>().CustomCalculateNewItemRowPosition -= OnCustomCalculateNewItemRowPosition;
        }
    }
}