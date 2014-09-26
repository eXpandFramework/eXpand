using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.ConditionalViewControlsPositioning {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Module.Captions.HeaderConditionalViewControlsPositioning, "1=1",
                "1=1", Module.Captions.ViewMessageConditionalViewControlsPositioning, Position.Bottom) { View = "ConditionalViewControlsPositioning_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Module.Captions.HeaderConditionalViewControlsPositioning, "1=1", "1=1",
                Module.Captions.HeaderConditionalViewControlsPositioning, Position.Top) { View = "ConditionalViewControlsPositioning_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ConditionalViewControlsPositioningForCustomerName, "1=1", "1=1", null, Position.DetailViewItem) { MessageProperty = "NameWarning", View = "ConditionalViewControlsPositioning_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ConditionalViewControlsPositioningForCustomerCity, "1=1", "1=1", null, Position.DetailViewItem) { MessageProperty = "CityWarning", View = "ConditionalViewControlsPositioning_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ConditionalViewControlsPositioning_DetailView");
            yield return new XpandNavigationItemAttribute("Additional View Controls/Conditional View Controls Positioning",
                "ConditionalViewControlsPositioning_DetailView");
            yield return new DisplayFeatureModelAttribute("ConditionalViewControlsPositioning_DetailView");
        }
    }
}
