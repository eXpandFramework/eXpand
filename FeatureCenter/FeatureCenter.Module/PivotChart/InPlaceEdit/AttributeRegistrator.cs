using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.InPlaceEdit
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        private const string InPlaceEdit_DetailView = "InPlaceEdit_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Analysis)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, InPlaceEdit_DetailView);
            yield return new XpandNavigationItemAttribute("PivotChart/In Place Edit", InPlaceEdit_DetailView) { ObjectKey = "Name='InPlaceEdit'" };
            yield return new DisplayFeatureModelAttribute(InPlaceEdit_DetailView, new BinaryOperator("Name", "InPlaceEdit"));
            
        }
    }
}
