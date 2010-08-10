using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace FeatureCenter.Module.Win.MasterDetail.AtAnyLevel
{
    public class OrdersNewItemRowPosition:ViewController<ListView>
    {
        public OrdersNewItemRowPosition() {
            TargetViewId = "MasterDetailAtAnyLevelOrder_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<NewItemRowListViewController>().CustomCalculateNewItemRowPosition+=OnCustomCalculateNewItemRowPosition;
        }


        void OnCustomCalculateNewItemRowPosition(object sender, CustomCalculateNewItemRowPositionEventArgs customCalculateNewItemRowPositionEventArgs) {
            customCalculateNewItemRowPositionEventArgs.NewItemRowPosition=NewItemRowPosition.Top;

        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.GetController<NewItemRowListViewController>().CustomCalculateNewItemRowPosition -= OnCustomCalculateNewItemRowPosition;
        }
    }
}
