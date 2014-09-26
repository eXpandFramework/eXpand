using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.ModelArtifact.ConditionalSaveDelete {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderConditionalSaveDelete, "1=1", "1=1", Captions.ViewMessageConditionalSaveDelete, Position.Bottom) { View = "ConditionalSaveDelete_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderConditionalSaveDelete, "1=1", "1=1", Captions.HeaderConditionalSaveDelete, Position.Top) { View = "ConditionalSaveDelete_DetailView" };
            yield return new ActionStateRuleAttribute("Disable Save for ConditionalSaveDelete", "Save", "City='Paris'", "1=1", ActionState.Disabled) { View = "ConditionalSaveDelete_DetailView", ExecutionContextGroup = "ConditionalSaveDelete" };
            yield return new ActionStateRuleAttribute("Disable SaveAndClose for ConditionalSaveDelete", "SaveAndClose", "City='Paris'", "1=1", ActionState.Disabled) { ExecutionContextGroup = "ConditionalSaveDelete", View = "ConditionalSaveDelete_DetailView" };
            yield return new ActionStateRuleAttribute("Disable Delete for ConditionalSaveDelete", "Delete", "City='Paris'", "1=1", ActionState.Disabled) { ExecutionContextGroup = "ConditionalSaveDelete", View = "ConditionalSaveDelete_DetailView" };
            yield return new XpandNavigationItemAttribute("ModelArtifact/Conditional Save Delete", "ConditionalSaveDelete_DetailView");
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ConditionalSaveDelete_DetailView");
            yield return new DisplayFeatureModelAttribute("ConditionalSaveDelete_DetailView");
        }
    }
}
