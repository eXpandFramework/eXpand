using DevExpress.Xpo;
using eXpand.ExpressApp.MasterDetail.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.MasterDetail {
    [MasterDetail("MDOrder_OrderLines", "1=1", "MDOrderLine_ListView", "OrderLines",View = "MDOrder_ListView")]
    public class MDOrder : OrderBase {
        MDCustomer _customer;

        public MDOrder(Session session) : base(session) {
        }

        [Association("MDOrder-MDOrderLines")]
        public XPCollection<MDOrderLine> OrderLines {
            get { return GetCollection<MDOrderLine>("OrderLines"); }
        }

        [Association("MDCustomer-MDOrders")]
        public MDCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (MDCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

    }
}