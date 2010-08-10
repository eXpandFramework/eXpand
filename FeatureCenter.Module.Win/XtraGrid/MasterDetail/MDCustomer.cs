using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.MasterDetail.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.MasterDetail {
    
    public class MDCustomer : CustomerBase {
        public MDCustomer(Session session) : base(session) {
        }

        [Association("MDCustomer-MDOrders")]
        public XPCollection<MDOrder> Orders {
            get { return GetCollection<MDOrder>("Orders"); }
        }

    }
}