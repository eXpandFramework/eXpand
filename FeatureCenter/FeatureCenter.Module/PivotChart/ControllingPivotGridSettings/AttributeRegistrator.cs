using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.ControllingPivotGridSettings
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        private const string ControllingPivotGridSettings_DetailView = "ControllingPivotGridSettings_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Analysis)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, ControllingPivotGridSettings_DetailView);
            yield return new XpandNavigationItemAttribute("PivotChart/Controlling Grid Settings", ControllingPivotGridSettings_DetailView) { ObjectKey = "Name='Controlling Grid Settings'" };
            yield return new DisplayFeatureModelAttribute(ControllingPivotGridSettings_DetailView, new BinaryOperator("Name", "Controlling Grid Settings"));
        }
    }
}
