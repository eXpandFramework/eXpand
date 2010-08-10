using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.ModelArtifact.ConditionalSaveDelete
{
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalSaveDelete, "1=1", "1=1", Captions.ViewMessageConditionalSaveDelete, Position.Bottom, ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalSaveDelete, "1=1", "1=1", Captions.HeaderConditionalSaveDelete, Position.Top, ViewType = ViewType.ListView)]
//    [ActionStateRule("Disable Save for ConditionalSaveDelete", "Save", "City='Paris'", "1=1", ActionState.Disabled, ExecutionContextGroup = "ConditionalActionContextGroup")]
//    [ActionStateRule("Disable SaveAndClose for ConditionalSaveDelete", "SaveAndClose", "City='Paris'", "1=1", ActionState.Disabled, ExecutionContextGroup = "ConditionalActionContextGroup")]
//    [ActionStateRule("Disable Delete for ConditionalSaveDelete", "Delete", "City='Paris'", "1=1", ActionState.Disabled, ExecutionContextGroup = "ConditionalActionContextGroup")]
    public class CSDCustomer:CustomerBase
    {
        public CSDCustomer(Session session) : base(session) {
        }

    }
}
