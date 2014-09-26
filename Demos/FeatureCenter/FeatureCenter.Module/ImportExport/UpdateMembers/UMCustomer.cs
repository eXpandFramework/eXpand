using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Module.ImportExport.UpdateMembers {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderUpdateMembers, "1=1", "1=1", Captions.ViewMessageUpdateMembers, Position.Bottom, ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderUpdateMembers, "1=1", "1=1", Captions.HeaderUpdateMembers, Position.Top)]
    [XpandNavigationItem(Captions.Importexport + "Update Members", "UMCustomer_ListView")]
    public class UMCustomer : CustomerBase {
        public UMCustomer(Session session)
            : base(session) {
        }
    }
}
