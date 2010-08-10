using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.XtraGrid
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + BaseObjects.Captions.HeaderControlXtraGrid, "1=1", "1=1",
        BaseObjects.Captions.ViewMessageControlXtraGrid, Position.Bottom)]
    [AdditionalViewControlsRule(Captions.Header + " " + BaseObjects.Captions.HeaderControlXtraGrid, "1=1", "1=1",
        BaseObjects.Captions.HeaderControlXtraGrid, Position.Top)]
    public class CXRCustomer:CustomerBase
    {
        public CXRCustomer(Session session) : base(session) {
        }

    }
}
