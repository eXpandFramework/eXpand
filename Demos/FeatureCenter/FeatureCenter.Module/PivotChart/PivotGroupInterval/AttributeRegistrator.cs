using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.PivotChart.PivotGroupInterval {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string DetailView = "PivotGroupInterval_DetailView1";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Analysis)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, DetailView);
            yield return new XpandNavigationItemAttribute("PivotChart/Pivot Group Interval", DetailView) { ObjectKey = "Name='PivotGroupInterval'" };
            yield return new DisplayFeatureModelAttribute(DetailView, new BinaryOperator("Name", "PivotGroupInterval"));
            yield return new ActionStateRuleAttribute("Hide_save_and_close_for_" + DetailView, "SaveAndClose", "1=1", "1=1", ActionState.Hidden);
        }
    }
}
