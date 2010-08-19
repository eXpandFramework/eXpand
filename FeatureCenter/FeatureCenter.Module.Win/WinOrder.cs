using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win
{
    
    public class WinOrder : OrderBase {
        WinCustomer _winCustomer;

        public WinOrder(Session session) : base(session) {
        }

        [Association("WinOrder-WinOrderLines")]
        public XPCollection<WinOrderLine> OrderLines {
            get { return GetCollection<WinOrderLine>("OrderLines"); }
        }

        [Association("WinCustomer-WinOrders")]
        public WinCustomer Customer {
            get { return _winCustomer; }
            set { SetPropertyValue("Customer", ref _winCustomer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (WinCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }
    }
}