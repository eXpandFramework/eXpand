using System;
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.MasterDetail.ConditionalDetailViews {

    public class MDCDVOrder : OrderBase {
        MDCDVCustomer _customer;

        public MDCDVOrder(Session session) : base(session) {
        }

        [Association("MDCDVCustomer-MDCDVOrders")]
        public MDCDVCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        [Association("MDCDVOrder-MDCDVOrderLines")]
        public XPCollection<MDCDVOrderLine> OrderLines {
            get { return GetCollection<MDCDVOrderLine>("OrderLines"); }
        }
        protected override void SetCustomer(ICustomer customer) {
            Customer = (MDCDVCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            throw new NotImplementedException();
        }

    }
}