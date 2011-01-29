using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win {
    [DefaultClassOptions]
    public class WinCustomer : CustomerBase {
        public WinCustomer(Session session)
            : base(session) {
        }

        [Association("WinCustomer-WinOrders")]
        public XPCollection<WinOrder> Orders {
            get { return GetCollection<WinOrder>("Orders"); }
        }
    }
}