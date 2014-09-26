using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.PivotChart.HidePivot {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string DetailView = "HidePivot_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Analysis)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, DetailView);
            yield return new XpandNavigationItemAttribute("PivotChart/Hide Pivot", DetailView) { ObjectKey = "Name='HidePivot'" };
            yield return new DisplayFeatureModelAttribute(DetailView, new BinaryOperator("Name", "HidePivot"));
            yield return new ActionStateRuleAttribute("Hide_save_and_close_for_" + DetailView, "SaveAndClose", "1=1", "1=1", ActionState.Hidden);
        }
    }
}
