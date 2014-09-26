using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.PivotChart.ControllingPivotGridSettings {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string ControllingPivotGridSettings_DetailView = "ControllingPivotGridSettings_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Analysis)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, ControllingPivotGridSettings_DetailView);
            yield return new XpandNavigationItemAttribute("PivotChart/Controlling Grid Settings", ControllingPivotGridSettings_DetailView) { ObjectKey = "Name='Controlling Grid Settings'" };
            yield return new DisplayFeatureModelAttribute(ControllingPivotGridSettings_DetailView, new BinaryOperator("Name", "Controlling Grid Settings"));
            yield return new ActionStateRuleAttribute("Hide_save_and_close_for_" + ControllingPivotGridSettings_DetailView, "SaveAndClose", "1=1", "1=1", ActionState.Hidden);
        }
    }
}
