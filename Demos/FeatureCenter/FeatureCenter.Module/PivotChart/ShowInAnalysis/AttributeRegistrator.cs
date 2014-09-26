using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.PivotChart.ShowInAnalysis {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(OrderLine)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderShowInAnalysis, "1=1", "1=1", Captions.ViewMessageShowInAnalysis, Position.Bottom) { View = "ShowInAnalysis_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderShowInAnalysis, "1=1", "1=1", Captions.HeaderShowInAnalysis, Position.Top) { View = "ShowInAnalysis_ListView" };
            yield return new XpandNavigationItemAttribute("PivotChart/Show In Analysis", "ShowInAnalysis_ListView");
            yield return new CloneViewAttribute(CloneViewType.ListView, "ShowInAnalysis_ListView");
            yield return new DisplayFeatureModelAttribute("ShowInAnalysis_ListView");
        }
    }
}
