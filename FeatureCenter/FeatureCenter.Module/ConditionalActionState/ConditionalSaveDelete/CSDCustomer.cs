using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using FeatureCenter.Module.BaseObjects;

namespace FeatureCenter.Module.ConditionalActionState.ConditionalSaveDelete
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalSaveDelete, "1=1", "1=1", Captions.ViewMessageConditionalSaveDelete, Position.Bottom, ViewType = ViewType.ListView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalSaveDelete, "1=1", "1=1", Captions.HeaderConditionalSaveDelete, Position.Top, ViewType = ViewType.ListView)]
    [ConditionalActionStateRule("Disable Save for ConditionalSaveDelete", "Save", "City='Paris'", "1=1", ActionState.Disabled, ExecutionContextGroup =  "ModelArtifactContext")]
    [ConditionalActionStateRule("Disable Delete for ConditionalSaveDelete", "Delete", "City='Paris'", "1=1", ActionState.Disabled,ExecutionContextGroup = "ModelArtifactContext")]
    public class CSDCustomer:CustomerBase
    {
        public CSDCustomer(Session session) : base(session) {
        }

        protected override IEnumerable<IOrder> GetOrders() {
            throw new NotImplementedException();
        }
    }
}
