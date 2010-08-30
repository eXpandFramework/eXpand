using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.MasterDetail.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.MasterDetail.AutoExpandNewRow {
    
    public class AENRCustomer : CustomerBase {
        public AENRCustomer(Session session) : base(session) {
        }

        [Association("AENRCustomer-AENROrders")]
        public XPCollection<AENROrder> AENROrders
        {
            get { return GetCollection<AENROrder>("AENROrders"); }
        }

    }
}