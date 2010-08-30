using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.AllowEditDetailView
{
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderDisableEditDetailView, "1=1", "1=1",
//        Captions.ViewMessageDisableEditDetailView, Position.Bottom)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderDisableEditDetailView, "1=1", "1=1",
//        Captions.HeaderDisableEditDetailView, Position.Top)]
    public class DEDVCustomer:CustomerBase
    {
        public DEDVCustomer(Session session) : base(session) {
        }

    }
}
