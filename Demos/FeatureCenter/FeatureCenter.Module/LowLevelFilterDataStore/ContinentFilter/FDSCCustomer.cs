using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.LowLevelFilterDataStore.ContinentFilter {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderFilterDataStoreContinentFilter, "1=1", "1=1", Captions.ViewMessageFilterDataStoreContinentFilter, Position.Bottom)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderFilterDataStoreContinentFilter, "1=1", "1=1", Captions.HeaderFilterDataStoreContinentFilter, Position.Top)]
    [XpandNavigationItem("Low Level Filter DataStore/Filter By Continent", "FeatureCenter.Module.LowLevelFilterDataStore.ContinentFilter.FDSCCustomer_ListView")]
    [DisplayFeatureModel("FeatureCenter.Module.LowLevelFilterDataStore.ContinentFilter.FDSCCustomer_ListView", "ContinentFilter")]
    public class FDSCCustomer:CustomerBase {
        public FDSCCustomer(Session session) : base(session) {
        }

    }
}