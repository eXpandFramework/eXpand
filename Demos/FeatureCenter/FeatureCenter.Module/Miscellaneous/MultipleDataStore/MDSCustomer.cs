using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;
using FeatureCenter.Module.Miscellaneous.MultipleDataStore;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;

[assembly :DataStore(typeof(MDSCustomer),"FeatureCenterMultipleDataStore")]
namespace FeatureCenter.Module.Miscellaneous.MultipleDataStore {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderMultipleDataStores, "1=1", "1=1", Captions.ViewMessageMultipleDataStores, Position.Bottom,ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderMultipleDataStores, "1=1", "1=1", Captions.HeaderMultipleDataStores, Position.Top)]
    [XpandNavigationItem(Captions.Miscellaneous + "Multiple DataStores", "MDSCustomer_ListView")]
    [DisplayFeatureModel("MDSCustomer_ListView", "MultipleDataStore")]
    public class MDSCustomer : CustomerBase {
        public MDSCustomer(Session session) : base(session) {
        }

    }
}