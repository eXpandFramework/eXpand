using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.FKViolation
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderFKViolation, "1=1", "1=1", Captions.ViewMessageFKViolation, Position.Bottom, ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderFKViolation, "1=1", "1=1", Captions.HeaderFKViolation, Position.Top)]
    public class FKVCustomer:CustomerBase
    {
        public FKVCustomer(Session session) : base(session) {
        }

    }
}
