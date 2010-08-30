using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.MasterDetail.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.MasterDetail.ConditionalDetailViews
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + BaseObjects.Captions.HeaderConditionalDetailGridViews, "1=1", "1=1",
    BaseObjects.Captions.ViewMessageConditionalDetailGridViews, Position.Bottom, Nesting = Nesting.Root,
    ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + BaseObjects.Captions.HeaderConditionalDetailGridViews, "1=1", "1=1",
        BaseObjects.Captions.HeaderConditionalDetailGridViews, Position.Top, Nesting = Nesting.Root, ViewType = ViewType.ListView)]
    [MasterDetail("MDCDVCustomer_Orders_For_All_Other_Cities", "1=1", "MDCDVOrder_ListView", "Orders", View = "MDCDVCustomer_ListView")]
    public class MDCDVCustomer:CustomerBase
    {
        public MDCDVCustomer(Session session) : base(session) {
        }
        [Association("MDCDVCustomer-MDCDVOrders")]
        public XPCollection<MDCDVOrder> Orders
        {
            get
            {
                return GetCollection<MDCDVOrder>("Orders");
            }
        }
    }
}
