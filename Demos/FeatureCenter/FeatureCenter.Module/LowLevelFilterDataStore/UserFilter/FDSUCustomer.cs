using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.LowLevelFilterDataStore.UserFilter {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderFilterDataStoreUserFilter, "1=1", "1=1", Captions.ViewMessageFilterDataStoreUserFilter, Position.Bottom)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderFilterDataStoreUserFilter, "1=1", "1=1", Captions.HeaderFilterDataStoreUserFilter, Position.Top)]
    [XpandNavigationItem("Low Level Filter DataStore/Filter By Current Logon User", "FDSUCustomer_ListView")]
    [DisplayFeatureModel("FDSUCustomer_ListView", "UserFilter")]
    public class FDSUCustomer : CustomerBase {
        public FDSUCustomer(Session session)
            : base(session) {
        }

    }
}
