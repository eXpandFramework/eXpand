using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using eXpand.Xpo.Attributes;
using FeatureCenter.Base;
using FeatureCenter.Module.MultipleDataStore;

[assembly :DataStore(typeof(MDSCustomer),"FeatureCenterMultipleDataStore")]
namespace FeatureCenter.Module.MultipleDataStore {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderMultipleDataStores, "1=1", "1=1", Captions.ViewMessageMultipleDataStores, Position.Bottom,ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderMultipleDataStores, "1=1", "1=1", Captions.HeaderMultipleDataStores, Position.Top)]
    [NavigationItem(Captions.Miscellaneous+"Multiple DataStores", "MDSCustomer_ListView")]
    [DisplayFeatureModel("MDSCustomer_ListView", "MultipleDataStore")]
    public class MDSCustomer : CustomerBase {
        public MDSCustomer(Session session) : base(session) {
        }

    }
}