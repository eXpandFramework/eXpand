using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.PivotChart.InPlaceEdit {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string InPlaceEdit_DetailView = "InPlaceEdit_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Analysis)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, InPlaceEdit_DetailView);
            yield return new XpandNavigationItemAttribute("PivotChart/In Place Edit", InPlaceEdit_DetailView) { ObjectKey = "Name='InPlaceEdit'" };
            yield return new DisplayFeatureModelAttribute(InPlaceEdit_DetailView, new BinaryOperator("Name", "InPlaceEdit"));
            yield return new ActionStateRuleAttribute("Hide_save_and_close_for_" + InPlaceEdit_DetailView, "SaveAndClose", "1=1", "1=1", ActionState.Hidden);
        }
    }
}
