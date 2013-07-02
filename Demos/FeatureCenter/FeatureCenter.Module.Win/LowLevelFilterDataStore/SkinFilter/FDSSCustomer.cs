using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.LowLevelFilterDataStore.SkinFilter {

    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Module.Captions.HeaderFilterDataStoreSkinFilter, "1=1", "1=1", Module.Captions.ViewMessageFilterDataStoreSkinFilter, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Module.Captions.HeaderFilterDataStoreSkinFilter, "1=1", "1=1", Module.Captions.HeaderFilterDataStoreSkinFilter, Position.Top)]
    [XpandNavigationItem("Low Level Filter DataStore/Filter By Application Skin", "FeatureCenter.Module.Win.LowLevelFilterDataStore.SkinFilter.FDSSCustomer_ListView")]
    [DisplayFeatureModel("FeatureCenter.Module.Win.LowLevelFilterDataStore.SkinFilter.FDSSCustomer_ListView", "SkinFilter")]
    public class FDSSCustomer : CustomerBase {
        public FDSSCustomer(Session session)
            : base(session) {
        }

    }
}
