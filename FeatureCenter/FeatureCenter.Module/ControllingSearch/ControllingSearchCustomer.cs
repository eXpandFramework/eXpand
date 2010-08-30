using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.ControllingSearch
{
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderControllingListViewSearch, "1=1", "1=1", Captions.ViewMessageControllingListViewSearch, Position.Bottom,ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderControllingListViewSearch, "1=1", "1=1", Captions.HeaderControllingListViewSearch, Position.Top,ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderControllingDetailViewSearch, "1=1", "1=1", Captions.ViewMessageControllingDetailViewSearch, Position.Bottom,ViewType = ViewType.DetailView)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderControllingDetailViewSearch, "1=1", "1=1", Captions.HeaderControllingDetailViewSearch, Position.Top,ViewType = ViewType.DetailView)]
    public class ControllingSearchCustomer:CustomerBase
    {
        public ControllingSearchCustomer(Session session) : base(session) {
        }

        [Association("ControllingSearchCustomer-ControllingSearchOrders")]
        public XPCollection<ControllingSearchOrder> Orders {
            get { return GetCollection<ControllingSearchOrder>("Orders"); }
        }
    }
}
