using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module {
    #region Detail View of Persistent object with records
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderShowInAnalysis, "1=1", "1=1", Captions.ViewMessageShowInAnalysis, Position.Bottom, View = "ShowInAnalysis_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderShowInAnalysis, "1=1", "1=1", Captions.HeaderShowInAnalysis, Position.Top, View = "ShowInAnalysis_ListView")]

    [eXpand.ExpressApp.Attributes.NavigationItem("PivotChart/Show In Analysis", "ShowInAnalysis_ListView")]
    [CloneView(CloneViewType.ListView, "ShowInAnalysis_ListView")]
    [DisplayFeatureModelAttribute("ShowInAnalysis_ListView")]
    #endregion
    [VisibleInReports(true)]
    public class OrderLine : OrderLineBase {
        Order _order;

        public OrderLine(Session session) : base(session) {
        }

        [Association("Order-OrderLines")]
        public Order Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }

        protected override void SetOrder(IOrder order) {
            Order = (Order) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}