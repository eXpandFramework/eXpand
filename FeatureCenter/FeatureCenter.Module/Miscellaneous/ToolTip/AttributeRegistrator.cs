using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Miscellaneous.ToolTip {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string Tooltip_DetailView = "Tooltip_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (!Object.Equals(typesInfo.Type, typeof (Customer))) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderTooltip, "1=1", "1=1", Captions.ViewMessageTooltip, Position.Bottom) { ViewType = ViewType.DetailView, View = Tooltip_DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderTooltip, "1=1", "1=1", Captions.HeaderTooltip, Position.Top) { View = Tooltip_DetailView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, Tooltip_DetailView);
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute(Captions.Miscellaneous + "Tooltips", Tooltip_DetailView);
            yield return xpandNavigationItemAttribute;
            yield return new DisplayFeatureModelAttribute(Tooltip_DetailView);
            yield return new WhatsNewAttribute(new DateTime(2011, 2, 22), xpandNavigationItemAttribute);
        }
    }
}
