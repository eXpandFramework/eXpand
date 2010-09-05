using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.AdditionalViewControls.ConditionalControlAndMessage
{
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(Customer))yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderConditionalControlAndMessage, "1=1", "1=1",
        Captions.HeaderConditionalControlAndMessage, Position.Top){View = "ConditionalControlAndMessage_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.ConditionalAdditionalViewControlAndMessage, "Orders.Count>7", "1=1", null,
        Position.Bottom){ViewType = ViewType.ListView,MessageProperty = "ConditionalControlAndMessage",View = "ConditionalControlAndMessage_ListView"};
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderConditionalControlAndMessage, "1=1", "1=1",
        Captions.ViewMessageAdditionalViewControls, Position.Bottom){ ViewType = ViewType.ListView,View = "ConditionalControlAndMessage_ListView",
        ExecutionContextGroup = "ConditionalControlAndMessage"};
            yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalControlAndMessage_ListView");
            yield return new XpandNavigationItemAttribute("Additional View Controls/Conditional control with conditional Message","ConditionalControlAndMessage_ListView");
            yield return new DisplayFeatureModelAttribute("ConditionalControlAndMessage_ListView");
        }
    }
}
