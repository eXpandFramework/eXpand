using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.HideToolBar
{
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderHideListViewToolBar, "1=1", "1=1", Captions.ViewMessageHideListViewToolBar, Position.Bottom, ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderHideListViewToolBar, "1=1", "1=1", Captions.HeaderHideListViewToolBar, Position.Top,ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderHideDetailViewToolBar, "1=1", "1=1", Captions.ViewMessageHideDetailViewToolBar, Position.Bottom, ViewType = ViewType.DetailView)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderHideDetailViewToolBar, "1=1", "1=1", Captions.HeaderHideDetailViewToolBar, Position.Top, ViewType = ViewType.DetailView)]
    public class HTBCustomer:CustomerBase
    {
        public HTBCustomer(Session session) : base(session) {
        }

    }
}
