using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.PivotGroupInterval
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        private const string DetailView = "PivotGroupInterval_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Analysis)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, DetailView);
            yield return new XpandNavigationItemAttribute("PivotChart/Pivot Group Interval", DetailView) { ObjectKey = "Name='PivotGroupInterval'" };
            yield return new DisplayFeatureModelAttribute(DetailView, new BinaryOperator("Name", "PivotGroupInterval"));
            
        }
    }
}
