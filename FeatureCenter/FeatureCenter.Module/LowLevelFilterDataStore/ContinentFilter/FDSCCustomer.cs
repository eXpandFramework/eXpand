using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.LowLevelFilterDataStore.ContinentFilter {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderFilterDataStoreContinentFilter, "1=1", "1=1", Captions.ViewMessageFilterDataStoreContinentFilter, Position.Bottom)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderFilterDataStoreContinentFilter, "1=1", "1=1", Captions.HeaderFilterDataStoreContinentFilter, Position.Top)]
    [XpandNavigationItem("Low Level Filter DataStore/Filter By Continent", "FDSCCustomer_ListView")]
    [DisplayFeatureModel("FDSCCustomer_ListView", "ContinentFilter")]
    public class FDSCCustomer:CustomerBase {
        public FDSCCustomer(Session session) : base(session) {
        }

    }
}