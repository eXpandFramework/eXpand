using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.FilterDataStore.UserFilter
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderFilterDataStoreUserFilter, "1=1", "1=1", Captions.ViewMessageFilterDataStoreUserFilter, Position.Bottom)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderFilterDataStoreUserFilter, "1=1", "1=1", Captions.HeaderFilterDataStoreUserFilter, Position.Top)]
    [eXpand.ExpressApp.Attributes.NavigationItem("Low Level Filter DataStore/Filter By Current Logon User", "FDSUCustomer_ListView")]
    [DisplayFeatureModel("FDSUCustomer_ListView", "UserFilter")]
    public class FDSUCustomer:CustomerBase
    {
        public FDSUCustomer(Session session) : base(session) {
        }

    }
}
