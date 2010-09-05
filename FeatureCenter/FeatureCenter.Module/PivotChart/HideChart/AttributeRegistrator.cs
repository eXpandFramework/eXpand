using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.HideChart
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        private const string DetailView = "HideChart_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Analysis)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, DetailView);
            yield return new XpandNavigationItemAttribute("PivotChart/Hide Chart", DetailView) {ObjectKey = "Name='HideChart'"};
            yield return new DisplayFeatureModelAttribute(DetailView, new BinaryOperator("Name", "HideChart"));
        }
    }
}
