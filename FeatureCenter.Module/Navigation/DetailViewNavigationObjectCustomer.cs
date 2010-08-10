using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Navigation {
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderDetailViewNavigation, "1=1", "1=1", Captions.HeaderDetailViewNavigation, Position.Top)]
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderDetailViewNavigation, "1=1", "1=1", Captions.ViewMessageDetailViewNavigation, Position.Bottom,ViewType = ViewType.DetailView)]
    public class DetailViewNavigationObjectCustomer:CustomerBase {
        public DetailViewNavigationObjectCustomer(Session session) : base(session) {
        }

    }
}