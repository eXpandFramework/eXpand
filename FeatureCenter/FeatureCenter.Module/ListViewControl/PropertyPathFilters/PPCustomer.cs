using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.ListViewControl.PropertyPathFilters {
    [XpandNavigationItem(Captions.ListViewCotrol + "Property Path Filters", "PPCustomer_ListView")]
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderPropertyPathFilters, "1=1", "1=1",Captions.ViewMessagePropertyPathFilters, Position.Bottom,ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderPropertyPathFilters, "1=1", "1=1", Captions.HeaderPropertyPathFilters, Position.Top, ViewType = ViewType.ListView)]
    public class PPCustomer : CustomerBase {
        public PPCustomer(Session session) : base(session) {
        }

        [Association("PPCustomer-Orders")]
        public XPCollection<PPOrder> Orders {
            get { return GetCollection<PPOrder>("Orders"); }
        }

    }
}