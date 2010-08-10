using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.XtraGrid {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + BaseObjects.Captions.HeaderControlXtraGridColumns, "1=1", "1=1",
        BaseObjects.Captions.ViewMessageControlXtraGridColumns, Position.Bottom)]
    [AdditionalViewControlsRule(Captions.Header + " " + BaseObjects.Captions.HeaderControlXtraGridColumns, "1=1", "1=1",
        BaseObjects.Captions.HeaderControlXtraGridColumns, Position.Top)]
    public class CXRCCustomer:CustomerBase
    {
        public CXRCCustomer(Session session)
            : base(session)
        {
        }

    }
}