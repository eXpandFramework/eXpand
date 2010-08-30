using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win {
    public class WinCustomer : CustomerBase {
        public WinCustomer(Session session) : base(session) {
        }

        [Association("WinCustomer-WinOrders")]
        public XPCollection<WinOrder> Orders {
            get { return GetCollection<WinOrder>("Orders"); }
        }
    }
}