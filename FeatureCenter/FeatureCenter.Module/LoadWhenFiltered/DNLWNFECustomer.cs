using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.LoadWhenFiltered
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderLoadWhenFiltered, "1=1", "1=1", Captions.ViewMessageLoadWhenFiltered, Position.Bottom, ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderLoadWhenFiltered, "1=1", "1=1", Captions.HeaderLoadWhenFiltered, Position.Top, ViewType = ViewType.ListView)]
    public class DNLWNFECustomer:CustomerBase
    {
        public DNLWNFECustomer(Session session) : base(session) {
        }

    }
}
