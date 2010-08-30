using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.ConditionalControllerState.Logic;
using eXpand.ExpressApp.SystemModule;
using FeatureCenter.Base;

namespace FeatureCenter.Module.ModelArtifact.ConditionalFKViolationAndShowDetailView
{
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1", Captions.ViewMessageConditionalFKViolation, Position.Bottom, ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation+"2", "1=1", "1=1", Captions.ViewMessageConditionalFKViolation, Position.Bottom, ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1", Captions.HeaderConditionalFKViolation, Position.Top, ViewType = ViewType.ListView)]
//    [ControllerStateRule("ConditionalFKViolation", typeof(FKViolationViewController), "City='Paris'", "1=1", ControllerState.Disabled, ExecutionContextGroup = "ConditionalControllerContextGroup")]
//    [ControllerStateRule("ConditionalShowDetailView", typeof(ListViewProcessCurrentObjectController), "City='Paris'", "1=1", ControllerState.Disabled, ExecutionContextGroup = "ConditionalControllerContextGroup")]
    public class CFKVCustomer:CustomerBase
    {
        public CFKVCustomer(Session session) : base(session) {
        }

    }

    
}
