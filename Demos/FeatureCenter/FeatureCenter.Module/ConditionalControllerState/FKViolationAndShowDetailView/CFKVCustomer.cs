using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.ConditionalControllerState.Logic;
using eXpand.ExpressApp.SystemModule;
using FeatureCenter.Module.BaseObjects;

namespace FeatureCenter.Module.ConditionalControllerState.FKViolationAndShowDetailView
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1", Captions.ViewMessageConditionalFKViolation, Position.Bottom, ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation+"2", "1=1", "1=1", Captions.ViewMessageConditionalFKViolation, Position.Bottom, ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1", Captions.HeaderConditionalFKViolation, Position.Top, ViewType = ViewType.ListView)]
    [ControllerStateRule("ConditionalFKViolation", typeof(FKViolationViewController), "City='Paris'", "1=1", ControllerState.Disabled, ExecutionContextGroup = "ModelArtifactContext")]
    [ControllerStateRule("ConditionalShowDetailView", typeof(ListViewProcessCurrentObjectController), "City='Paris'", "1=1", ControllerState.Disabled, ExecutionContextGroup = "ModelArtifactContext")]
    public class CFKVCustomer:CustomerBase
    {
        public CFKVCustomer(Session session) : base(session) {
        }

        protected override IEnumerable<IOrder> GetOrders() {
            throw new NotImplementedException();
        }
    }

    
}
